namespace Logger.Core;

/// <summary>
/// The closed set of data types a log field may declare (REQ-0002).
/// Kept closed so a schema cannot declare an unknown type. When field types are
/// ever parsed from external input, validate with <see cref="System.Enum.IsDefined{TEnum}(TEnum)"/>
/// to guard against out-of-range casts (ISO/IEC 24772-1 [CCB]).
/// </summary>
public enum FieldType
{
    Time,
    IpAddress,
    String,
    Integer
}
