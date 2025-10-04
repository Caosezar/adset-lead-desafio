namespace AdsetManagement.Domain.ValueObjects;

public record OtherOptions
{
    public bool? ArCondicionado { get; init; }
    public bool? Alarme { get; init; }
    public bool? Airbag { get; init; }
    public bool? ABS { get; init; }

    public static OtherOptions Create(bool? arCondicionado = null, bool? alarme = null, bool? airbag = null, bool? abs = null)
    {
        return new OtherOptions
        {
            ArCondicionado = arCondicionado,
            Alarme = alarme,
            Airbag = airbag,
            ABS = abs
        };
    }
}