using System.Windows.Forms;

using System.Text.Json;
using System.Text.Json.Serialization;

using RecBinderProject.Domain.MacroEvents;
using RecBinderProject.Domain.MacroEvents.Enums;


namespace RecBinderProject.Infrastructure.Serialization
{
    public class MacroEventJsonConverter : JsonConverter<MacroEvent>
    {
        public override MacroEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            JsonElement root = document.RootElement;

            MacroEventKind kind = (MacroEventKind)root.GetProperty("Kind").GetByte();
            int timeOffset = root.GetProperty("TimeOffset").GetInt32();

            return kind switch
            {
                MacroEventKind.KeyboardPress => MacroEvent.FromKeyboardPress(root.GetProperty("KeyCode").GetInt32(), (MacroKeyState)root.GetProperty("KeyState").GetByte(), timeOffset),

                MacroEventKind.MouseMove => MacroEvent.FromMouseMove(root.GetProperty("X").GetDouble(), root.GetProperty("Y").GetDouble(), timeOffset),

                MacroEventKind.MousePress => MacroEvent.FromMousePress((MouseButtons)root.GetProperty("MouseButton").GetInt32(), (MacroKeyState)root.GetProperty("KeyState").GetByte(), timeOffset),

                MacroEventKind.MouseWheel => MacroEvent.FromMouseWheel(root.GetProperty("Delta").GetInt32(), timeOffset),

                _ => throw new JsonException($"Unknown event kind: {kind}")
            };
        }

        public override void Write(Utf8JsonWriter writer, MacroEvent value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("Kind", (byte)value.Kind);
            writer.WriteNumber("TimeOffset", value.TimeOffset);

            switch (value.Kind)
            {
                case MacroEventKind.KeyboardPress:
                    writer.WriteNumber("KeyCode", value.KeyboardPress.KeyCode);
                    writer.WriteNumber("KeyState", (byte)value.KeyboardPress.KeyState);

                    break;

                case MacroEventKind.MousePress:
                    writer.WriteNumber("MouseButton", (int)value.MousePress.MouseButton);
                    writer.WriteNumber("KeyState", (byte)value.MousePress.KeyState);

                    break;

                case MacroEventKind.MouseMove:
                    writer.WriteNumber("X", value.MouseMove.X);
                    writer.WriteNumber("Y", value.MouseMove.Y);

                    break;

                case MacroEventKind.MouseWheel:
                    writer.WriteNumber("Delta", value.MouseWheel.Delta);

                    break;

                default: throw new JsonException($"Unknown event kind: {value.Kind}");
            }

            writer.WriteEndObject();
        }
    }
}
