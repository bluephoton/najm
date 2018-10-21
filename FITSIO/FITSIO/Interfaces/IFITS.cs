using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Najm.LinearAlagebra;

namespace Najm.FITSIO
{
    public enum HDUType { Unknown, Primary, Image, ASCIITable, BinaryTable, RandomGroup }
    public enum CardImageType { Blank, KeyValue, Comment, History, End, Unknown }
    public enum FieldType { Bool = 'L', Bits = 'X', Byte = 'B', Int16 = 'I', Int32 = 'J', Int64 = 'K', Char = 'A', Float = 'E', Double = 'D', ComplexF = 'C', ComplexD = 'M', Invalid }
    public enum TableType { ASCII, Binary };

    public struct Complex<T>
    {
        public T _r;
        public T _i;
    }

    public interface ICellValue<T>
    {
        int Depth { get; }
        T this[int index] { get; }
        T[] Vector { get; }
    }

    public interface ITableCell
    {
        int Depth { get; }
        void get<T>(out ICellValue<T> val);
        T Value<T>();
    }

    public interface ITableRow : IEnumerable<ITableCell>
    {
        ITableCell this[int index] { get; }
        ITableCell this[string name] { get; }
    }

    public interface IFieldInfo
    {
        string Name { get; }
        int Index { get; }
        long Size { get; }
        double Zero { get; }
        double Scale { get; }
        string NULL { get; }    // i use string so that it works for both binary and ascii tables
                                // side effect is that you need to convert to int when needed
        string Unit { get; }
        string Format { get; }
        string DisplayFormat { get; }
        FieldType Type { get; }
        ITableHeader Header { get; }
    }

    public interface ITableHeader : IEnumerable<IFieldInfo>
    {
        int FieldsCount { get; }
        IFieldInfo this[int index] { get; }
        IFieldInfo this[string name] { get; }
        ITable Table { get; }
    }

    public interface ITable : IEnumerable<ITableRow>
    {
        ITableHeader Header { get; }
        ITableRow this[long index] { get; }
        long Length { get; }
        IHDU HDU { get; }
        TableType Type { get; }
    }

    public interface ICardImage
    {
        string Key { get; set; }
        string Value { get; set; }
        string Comment { get; set; }
        string InfoText { get; }
        void Update(string content);
        void Update(string key, string value);
        void Update(string key, string value, string comment);
        bool IsEmpty { get; }
        bool IsComment { get; }
        bool IsHistory { get; }
        bool IsWCS { get; }
        bool IsTable { get; }
        bool IsOneIndexForm(string startTag);
        bool IsTwoIndexForm(string startTag);
        ICardImage Copy();
    }

    public interface IHeader
    {
        ICardImage[] CardImages { get; }
        ICardImage this[int index] { get; }
        ICardImage this[string name] { get; }
        string FullText { get; }
        string SummaryText { get; }
        IWCSInfo WCSInfo { get; }
    }

    public interface IDataManager
    {
        int BitsPerPixel { get; }
        double BlankValue { get; }
        byte[] RawData { get; }
        double[] Data { get; }
        long DataSize { get; }   // more effecient than Data.Length as it gets the size without actually allocating & unifying the data
        double Maximum { get; }
        double Minimum { get; }
        int NumSlices { get; }
        int CurrentSlice { get; set; }
        IHDU HDU { get; }
    }

    public interface IAxis
    {
        int Sequence { get; }
        int NumPoints { get; }
        string InfoText { get; }
    }

    public interface IHDU
    {
        int Index { get; }
        string Name { get; }
        HDUType Type { get; }
        bool HasData { get; }
        bool IsTable { get; }
        bool IsImage { get; }
        IHeader Header { get; }
        IAxis[] Axes { get; }
        IDataManager DataMngr { get; }
        ITable Table { get; }
        byte[] Heap { get; }
        IFITSFile File { get; }
    }

    public interface IFITSFile
    {
        void Load(string fileName);
        void Load(string fileName, bool readOnly);
        void Save();
        void Save(string fileName);
        string Name { get; }
        IHDU this[int index] { get; }
        IHDU this[string hduName] { get; }
        IHDU[] HDUs { get; }
        void Close();
    }
}
