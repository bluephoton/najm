---
layout: default
title: Najm - FITS Interface
---


```cs
namespace Najm.FITS
{
    public enum HDUTypes { Unknown, Primary, Image, ASCIITable, BinaryTable, RandomGroup }
    public enum CardImageType { Blank, KeyValue, Comment, History, End, Unknown }
    public enum FieldTypes { Bool = 'L', Bits = 'X', Byte = 'B', Int16 = 'I', Int32 = 'J', Int64 = 'K', Char = 'A', Float = 'E', Double = 'D', ComplexF = 'C', ComplexD = 'M' }

    public struct Complex<T>
    {
        public T _r;
        public T _i;
    }
 
    // Used to represent a bit. Internally we'll use BitArray
    // Idea is to do something like:
    //      ICellValue<Bit> bits;
    //      cell.get(out bits)
    public struct Bit {
        public Bit(bool val) { _val = val; }
        public bool Value { get { return _val; } }
        private bool _val;
    }

    public interface ICellValue<T>
    {
        int Depth { get; }
        T this[int index] { get; }
        T[] Vector { get; }
    }

    public interface ITableCell
    {
        void get<T>(out ICellValue<T> val);
        int Depth { get; }
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
        string NULL { get; }
        string Unit { get; }
        FieldTypes Type { get; }
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
    }

    public interface ICardImage
    {
        string Key { get; }
        string Value { get; }
        string Comment { get; }
        string InfoText { get; }
    }

    public interface IHeader
    {
        ICardImage[] CardImages { get; }
        ICardImage this[int index] { get; }
        ICardImage this[string name] { get; }
        string FullText { get; }
        string SummaryText { get; }
    }
 
    public interface IDataMngr
    {
        int BitsPerPixel { get; }
        byte[] RawData { get; }
        long RawDataSize { get; }   // more effecient than RawData.Length as it gets the size without actually loadins the data
        double[] Data { get; }
        long DataSize { get; }      // more effecient than Data.Length as it gets the size without actually loadins the data
        double BlankValue { get; }
        double Maximum { get; }
        double Minimum { get; }
        uint NumSlices { get; }
        uint CurrentSlice { get; set; }
        IHDU HDU { get; }
    }
 
    public interface IAxis
    {
        int Sequence { get; }
        int NumPoints { get; }
        string Type { get; }
        string InfoText { get; }
    }
 
    public interface IHDU
    {
        int Index { get; }
        string Name { get; }
        HDUTypes Type { get; }
        bool HasData { get; }
        bool IsTable { get; }
        bool IsImage { get; }
        IHeader Header { get; }
        IDataMngr DataMngr { get; }
        IAxis[] Axes { get; }
        byte[] Heap { get; }
        ITable Table { get; }
        IFITSFile File { get; }
    }

    public interface IFITSFile
    {
        void Load(string fileName);
        void Save();
        void Save(string fileName);
        string Name { get; }
        IHDU this[int index] { get; }
        IHDU this[string hduName] { get; }
        IHDU[] HDUs { get; }
        IntPtr Handle { get; }
    }
}
```