using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Najm.FITSIO
{
    class HDU : IHDU, IHeader
    {
        internal HDU(FITSFile file, int index)
        {
            _file = file;
            _index = index;
            _type = HDUType.Unknown;
            _bitsPerPixel = 0;
            _blank = double.NaN;
            _hasData = false;
            _dataMngr = null;
            _heap = null;
            _paramsLength = 0;
            _numGroups = 1;
            _table = null;
            _mayHaveExtensions = false;
            _cardImages = new List<CardImage>(20);
            _cardImagesMap = new Dictionary<string, int>(20);
            _axes = new List<Axis>();
            _wcsInfo = new WCSInfo(this);
        }

        internal void Load(FileStream fs)
        {
            // load card images
            ///////////////////////////////////////////////////////////////////
            LoadCardImages(fs);

            // verify that we already loaded something
            ///////////////////////////////////////////////////////////////////
            if (_cardImages.Count > 2)
            {
                // use card images to initialize things
                ///////////////////////////////////////////////////////////////////
                InitFromCardImages();

                // if this is a table HDU, tell the table to finalize its initialization (to basically build 
                // its field definition map and calculate field definition values that can be calculated only
                // when all field definitions have been read).
                ////////////////////////////////////////////////////////////////////////////////////////////////
                if (_table != null)
                {
                    _table.FinalizeConstruction();
                }

                // read the data based on the type of the HDU as follows:
                //	- CDataMngr (implements IData) is used for Primary and Image entension HDUs, That said,
                //	  it will be always created so that we can ask it for RawData, RawDataSize....etc
                //	- For binary and asci table, data will be passed to HDUTable (implements ITable) which
                //	  cracks the data structure and expose it though the ITable methods
                //	- For any other HDU type (Random group and Unknows), caller should call Raw* function on IDHU
                //	  to get access to the buffer of the data where he can interpret it himslef - that's why
                //	  I have Axes array available on both IHDU as well as IData.
                ////////////////////////////////////////////////////////////////////////////////////////////////
                CreateDataManager(fs);

                // If we don't have data in this HDU, we're done!
                ////////////////////////////////////////////////////////////////////////////////////////////////
                if (_hasData)
                {
                    InitializeHDUData();
                }
            }
        }

        private void CreateDataManager(FileStream fs)
        {
            // allocate for data
            ////////////////////////////////////////////////////////////////////////////////////////////////
            long rawDataSize = RawDataSize;
            byte[] data = new byte[rawDataSize];

            // load it from file
            ////////////////////////////////////////////////////////////////////////////////////////////////
            //TODO: dangerous cast we need to take care of this later
            fs.Read(data, 0, (int)rawDataSize);

            // find the number of slices in data
            ////////////////////////////////////////////////////////////////////////////////////////////////
            int numSlices = (_axes.Count > 2) ? _axes[2].NumPoints : 1;

            // create Data object based on data format
            ////////////////////////////////////////////////////////////////////////////////////////////////
            _dataMngr = new DataManager(this, data, _bitsPerPixel, _blank, numSlices);
        }

        private void InitializeHDUData()
        {
            switch (_type)
            {
                case HDUType.Primary:
                case HDUType.Image:
                    // process the data. will fix byte order, calculate extreme values, map to physical
                    _dataMngr.InitializeImage();
                    break;
                case HDUType.RandomGroup:
                case HDUType.Unknown:
                    // well, do nothing!
                    break;
                case HDUType.BinaryTable:
                // handle params that might appear after binary table;
                // fall throw
                case HDUType.ASCIITable:
                    _table.Data = _dataMngr.RawData;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    Utils.CheckBool(false, "invalid HDU Type");
                    break;
            }
        }

        internal long RawDataSize
        {
            get
            {
                long size = 0;
                if (_hasData)
                {
                    switch (_type)
                    {
                        case HDUType.Primary:
                        case HDUType.Image:
                            size = CalcArraySize();
                            break;
                        case HDUType.ASCIITable:
                            size = _table.DataSize;
                            break;
                        case HDUType.BinaryTable:
                            size = _table.DataSize;
                            size += (long)_paramsLength;
                            break;
                        case HDUType.RandomGroup:
                            size = CalcArraySize();
                            size += (long)_paramsLength;
                            break;
                        case HDUType.Unknown:
                            size = CalcArraySize();
                            size += (long)_paramsLength;
                            break;
                        default:
                            System.Diagnostics.Debug.Assert(false);
                            Utils.CheckBool(false, "Invalid HDU type encountered");
                            break;
                    }

                    // calc in bytes
                    size *= (long)((_bitsPerPixel > 0) ? _bitsPerPixel : -_bitsPerPixel);
                    size /= 8;

                    // in case we have GCOUNT != 1.
                    size *= (long)_numGroups;
                }
                return size;
            }
        }

        private long CalcArraySize()
        {
            long size = 1;
            foreach (Axis a in _axes)
            {
                // random groups structure allow NAXIS1 to be zero. I need to prevent this madness from causing a zero size
                int numPoints = a.NumPoints;
                if (numPoints == 0 && a.Sequence == 0)
                {
                    numPoints = 1;
                }
                size *= (long)numPoints;
            }
            return size;
        }

        private void LoadCardImages(FileStream fs)
        {
            while (fs.Position < fs.Length)
            {
                // create new card image object
                CardImage ci = new CardImage();

                // load it from stream
                ci.Load(fs);

                // We require that FIRST CardImage in any HDU Header must have key value. Noticed that
                // some files doesn't follow this which causes me to read forever untill bad things happen!
                if (_cardImages.Count == 0 && string.IsNullOrEmpty(ci.Key))
                {
                    // seek to end of file as we have no way to know what is coming. No card images added, so
                    // rest of the logic will end gracefully.			    
                    fs.Seek(0, SeekOrigin.End);
                    break;
                }

                // Maintain a vector to maintain order of items as inserted
                _cardImages.Add(ci);
                // append to our map of imagecards, If its already there don't add (comment key for example
                // is not unique!, so only first one will be returned if lookup with key)
                // also don't add blank cardimages
                if (ci.Type != CardImageType.Blank && !string.IsNullOrEmpty(ci.Key) && !_cardImagesMap.ContainsKey(ci.Key))
                {
                    _cardImagesMap.Add(ci.Key, _cardImages.Count - 1);
                }

                // check if it is the last one
                if (ci.Type == CardImageType.End)
                {
                    break;
                }
            }

            // if we hit eof, no need to do anything more, otherwise skip padding after cardimages
            //////////////////////////////////////////////////////////////////////////////////////
            if (fs.Position < fs.Length)
            {
                long numPaddigCardImages = _cardImages.Count % 36;
                numPaddigCardImages = (numPaddigCardImages != 0) ? 36 - numPaddigCardImages : 0;
                fs.Seek(numPaddigCardImages * 80, SeekOrigin.Current);
            }
        }

        private void InitFromCardImages()
        {
            // figure out the type of the HDU
            ///////////////////////////////////////////////////////////////////
            if (_cardImages[0].Key.StartsWith("SIMPLE"))
            {
                // primary
                // ensure we have expected card keys - this function check the common ones between primary
                // and extenstion. To avoid doublicate code
                VerifyMandatoryKeys();
                _type = HDUType.Primary;
                _name = "Primary";
            }
            else if (_cardImages[0].Key.StartsWith("XTENSION"))
            {
                // extension
                // verify that we have expected mandatory keys
                VerifyMandatoryKeys();
                int numAxes = int.Parse(_cardImages[2].Value);
                Utils.CheckBool(_cardImages[3 + numAxes].Key.StartsWith("PCOUNT") &&
                                _cardImages[3 + numAxes + 1].Key.StartsWith("GCOUNT"),
                                "missing 'PCOUNT' and 'GCOUNT' card images");

                // determine type of extension
                switch (_cardImages[0].Value)
                {
                    case "TABLE":
                    case "'TABLE'":
                        _type = HDUType.ASCIITable;
                        break;
                    case "BINTABLE":
                    case "'BINTABLE'":
                        _type = HDUType.BinaryTable;
                        break;
                    case "IMAGE":
                    case "'IMAGE'":
                        _type = HDUType.Image;
                        break;
                    default:
                        _type = HDUType.Unknown;
                        break;
                }
            }
            else
            {
                // what the heck!
                _type = HDUType.Unknown;
                Utils.CheckBool(false, "Unknown HDU type encountered");
            }

            // get relevant info from card images
            ///////////////////////////////////////////////////////////////////
            ProcessCardImages();
        }

        private void VerifyMandatoryKeys()
        {
            Utils.CheckBool(
                !string.IsNullOrEmpty(_cardImages[1].Key) && _cardImages[1].Key.StartsWith("BITPIX") &&
                !string.IsNullOrEmpty(_cardImages[2].Key) && _cardImages[2].Key.StartsWith("NAXIS"),
                              "'BITPIX' or 'NAXIS' were missing");
            int numAxes = int.Parse(_cardImages[2].Value);
            Utils.CheckBool(_cardImages.Count >= (3 + numAxes), "Invalid number of card images");
            for (int i = 1; i <= numAxes; i++)
            {
                int index = int.Parse(_cardImages[2 + i].Key.Substring(5));
                Utils.CheckBool(index == i, "bad NAXISn");
            }
        }

        private void ProcessCardImages()
        {
            // Loop on all card image and handle each case we find
            ///////////////////////////////////////////////////////////////////
            foreach (CardImage ci in _cardImages)
            {
                // skip blank card images
                if (ci.Type == CardImageType.Blank || string.IsNullOrEmpty(ci.Key))
                {
                    continue;
                }

                string key = ci.Key;
                string val = ci.Value;
                switch (key)
                {
                    // comment and history keys appear a lot and need no processing. so save time and skip them
                    case "COMMENT":
                    case "HISTORY":
                        break;
                    case "EXTNAME":
                    case "HDUNAME":
                        _name = val;
                        break;
                    case "BITPIX":
                        int iVal = int.Parse(val);
                        Utils.CheckBool(iVal == 8 || iVal == 16 || iVal == 32 || iVal == -32 || iVal == -64, "Invalid 'BITPIX'");
                        _bitsPerPixel = iVal;
                        break;
                    case "NAXIS":
                        int numAxes = int.Parse(val);
                        Utils.CheckBool(numAxes >= 0, "NAXIS can't have negative value");
                        _axes.Capacity = numAxes;
                        _hasData = (numAxes > 0);
                        break;
                    case "BLANK":
                        _blank = double.Parse(val);
                        break;
                    case "EXTEND":
                        _mayHaveExtensions = true;
                        // to supress warning, don't remember why i added it!!!
                        // TODO: check this later
                        _mayHaveExtensions = _mayHaveExtensions ? true:false;
                        break;
                    case "GROUPS":
                        // random groups are not supported. I'm just seeing if something for cheap can work
                        if (val == "T")
                        {
                            System.Diagnostics.Debug.Assert(false, "Random group encountered.");
                            Utils.CheckBool(_type == HDUType.RandomGroup, "HDU type should be a random group");
                        }
                        break;
                    case "PCOUNT":
                        // Although this will be used in the bintable extension, I'll catch it here as well since 
                        // it might appear with unknown extentions.
                        _paramsLength = uint.Parse(val);
                        break;
                    case "GCOUNT":
                        _numGroups = int.Parse(val);
                        break;
                    case "TFIELDS":
                        // by the time we receive this, the _CardImages array will have all info needed to 
                        // contruct the ASCII/Binary Table.
                        BuildTable(val);
                        break;
                    case "END":
                        // invoke FinalizeConstruction for WCS info. this will cause the IVector objects
                        // to be created from the dymaic arrays built during cardimages' processing.
                        _wcsInfo.FinalizeConstruction();
                        break;
                    default:
                        // if table is created, give the it the chance to consume card images
                        if (_table != null)
                        {
                            _table.HandleCardImage(ci);
                        }

                        // give the WCSInfo object the chance to consume card images
                        _wcsInfo.HandleCardImage(ci);

                        // handle NAXISn case
                        if(key.StartsWith("NAXIS"))
                        {
                            int axisSequence = int.Parse(key.Substring(5)) - 1; //-1 as index start from 1
                            int numPoints = int.Parse(val);
                            _axes.Add(new Axis(axisSequence, numPoints));
                            // any axis with zero points will indicate that we have no data
                            // EXCEPTION: random groups structure allow ONLY NAXIS1=0, so, I'll allow this too.
                            if (0 == numPoints)
                            {
                                if (axisSequence != 0)
                                {
                                    _hasData = false;
                                }
                                else
                                {
                                    _type = HDUType.RandomGroup;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void BuildTable(string val)
        {
            // check we have what we need. did i check for this already?!
            ///////////////////////////////////////////////////////////////////
            string[] cis = new string[] {"XTENSION", "BITPIX", "NAXIS", "NAXIS1", "NAXIS2", "PCOUNT", "GCOUNT"};
            for(int i = 0;i<cis.Length;i++)
            {
                Utils.CheckBool(_cardImages[i].Key.Equals(cis[i]), "Table extenstion doesn't have the requirted cardimages");
            }

            // we are fine, continue...
            ///////////////////////////////////////////////////////////////////
            long numBytesPerRow = long.Parse(_cardImages[3].Value);   // same for both ASCII and Binary table
            long numRows = long.Parse(_cardImages[4].Value);          // same for both ASCII and Binary table
            int numFields = int.Parse(val);                         // number of columns
            _paramsLength = uint.Parse(_cardImages[5].Value);        // Non zero for binary tables only
            uint numGroups = uint.Parse(_cardImages[6].Value);        // must be one for binary table
            Utils.CheckBool(numGroups == 1, "GCOUNT of Binary Table must be 1");

            // now we're ready for table creation
            ///////////////////////////////////////////////////////////////////
            TableType t = (_type == HDUType.ASCIITable) ? TableType.ASCII : TableType.Binary;
            _table = new HDUTable(this, t, numRows, numFields, numBytesPerRow);
            if (_paramsLength > 0)
            {
                _heap = new byte[_paramsLength];
            }
        }

        #region IHDU Members
        public int Index { get { return _index; } }
        public string Name { get { return _name; } }
        public HDUType Type { get { return _type; } }
        public bool HasData { get { return _hasData; } }
        public bool IsTable { get { return (_type == HDUType.ASCIITable || _type == HDUType.BinaryTable); } }
        public bool IsImage { get { return (_type == HDUType.Primary || _type == HDUType.Image); } }
        public IHeader Header { get { return this; } }
        public IDataManager DataMngr { get { return _dataMngr; } }
        public IAxis[] Axes { get { return _axes.ToArray(); } }
        public byte[] Heap { get { return _heap; } }
        public ITable Table { get { return _table; } }
        public IFITSFile File { get { return _file; } }
        private string TypeString {
            get {
                string type = "";
                switch (_type)
                {
                    case HDUType.Unknown:
                        type = "Unknown";
                        break;
                    case HDUType.Primary:
                        type = "Primary";
                        break;
                    case HDUType.Image:
                        type = "Image";
                        break;
                    case HDUType.ASCIITable:
                        type = "ASCII Table";
                        break;
                    case HDUType.BinaryTable:
                        type = "Binary Table";
                        break;
                    case HDUType.RandomGroup:
                        type = "Random Group";
                        break;
                    default:
                        Utils.CheckBool(false, "Invalid HDU type encountered");
                        break;
                }
                return type;
            }
        }
        #endregion

        #region IHeader Members

        public ICardImage[] CardImages { get { return _cardImages.ToArray(); } }
        public ICardImage this[int index] { get { return _cardImages[index]; } }
        public ICardImage this[string key] { get { return _cardImages[_cardImagesMap[key]]; } }
        public string FullText {
            get {
                StringBuilder sb = new StringBuilder(2000);
                foreach (CardImage ci in _cardImages)
                {
                    sb.Append(ci.InfoText).Append("\n");
                }
                return sb.ToString();
            }
        }

        public string SummaryText
        {
            get {
                string text = "";
                int naxis = (_axes != null) ? _axes.Count : 0;
                text = string.Format("Type:{0}\nBitsPerPixel={1}\nNAXIS={2}\n",
                    TypeString,
                    _bitsPerPixel,
                    naxis);
                if (_type == HDUType.Primary || _type == HDUType.Image || _type == HDUType.Unknown)
                {
                    for (int i = 0; i < naxis; i++)
                    {
                        text += _axes[i].InfoText;
                        text += "\n";
                    }
                }
                else if (_type == HDUType.ASCIITable || _type == HDUType.BinaryTable)
                {
                    text += String.Format("Rows:{0}\nColumns:{1}\n", Table.Length, Table.Header.FieldsCount);
                }
                return text;
            }
        }
        public IWCSInfo WCSInfo { get { return _wcsInfo; } }

        #endregion

        #region data members
        private int _index;
        private List<CardImage> _cardImages;              // to maintain order
        private Dictionary<string, int> _cardImagesMap;   // for by key lookup
        private HDUType _type;
        private string _name;
        private int _bitsPerPixel;
        private List<Axis> _axes;
        private bool _hasData;
        private double _blank;
        private bool _mayHaveExtensions;
        private uint _paramsLength;
        private int _numGroups;
        private HDUTable _table;
        private DataManager _dataMngr;
        private byte[] _heap;
        private FITSFile _file;
        private WCSInfo _wcsInfo;
        #endregion

    }
}
