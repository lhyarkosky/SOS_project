using System.Text.Json.Serialization;

namespace SOS_API.Models.Players
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PlayerType
    {
        Human,
        Computer
    }

    public interface IPlayer
    {
        string Name { get; set; }
        bool IsComputer { get; }
    }
}