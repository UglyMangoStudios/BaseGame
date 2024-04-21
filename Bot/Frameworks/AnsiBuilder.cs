using System.Text;

namespace SpaceDiscordBot.Frameworks
{

    /// <summary>
    /// Helper class that allows for colored text within Discord messages
    /// <br/> Note: only is visible on Desktop clients
    /// </summary>
    internal class AnsiBuilder
    {
        /// <summary>
        /// Helper class that stores different formats and styles into simple classes and codes
        /// </summary>
        internal abstract class AnsiBase
        {
            /// <summary>
            /// The code that represets this ansi effect
            /// </summary>
            public int Code { get; }
            protected AnsiBase(int code) => Code = code;
        }

        /// <summary>
        /// All ansi formats that are usable within Discord
        /// </summary>
        internal sealed class AnsiFormat : AnsiBase
        {
            public static readonly AnsiFormat Normal = new(0);
            public static readonly AnsiFormat Bold = new(1);
            public static readonly AnsiFormat Underline = new(4);

            private AnsiFormat(int code) : base(code) { }
        }

        /// <summary>
        /// All ansi background colors that are usable within Discord
        /// </summary>
        internal sealed class AnsiBackground : AnsiBase
        {
            public static readonly AnsiBackground FireflyDarkBlue = new(40);
            public static readonly AnsiBackground Orange = new(41);
            public static readonly AnsiBackground MarbleBlue = new(42);
            public static readonly AnsiBackground GrayTurquoise = new(43);
            public static readonly AnsiBackground Gray = new(44);
            public static readonly AnsiBackground Indigo = new(45);
            public static readonly AnsiBackground LightGray = new(46);
            public static readonly AnsiBackground White = new(47);


            private AnsiBackground(int code) : base(code) { }
        }

        /// <summary>
        /// All ansi text (aka foreground) colors that are usable within Discord
        /// </summary>
        internal sealed class AnsiForeground : AnsiBase
        {
            public static readonly AnsiForeground Gray = new(30);
            public static readonly AnsiForeground Red = new(31);
            public static readonly AnsiForeground Green = new(32);
            public static readonly AnsiForeground Yellow = new(33);
            public static readonly AnsiForeground Blue = new(34);
            public static readonly AnsiForeground Pink = new(35);
            public static readonly AnsiForeground Cyan = new(36);
            public static readonly AnsiForeground White = new(37);
            private AnsiForeground(int code) : base(code) { }
        }

        /// <summary>
        /// The prefix notation to designate ansi codes within a message
        /// </summary>
        private const string PREFIX = "\u001b[";

        /// <summary>
        /// Used to append lines of text to form a complete message
        /// </summary>
        private readonly StringBuilder stringBuilder = new();

        /// <summary>
        /// Append a line of text to the builder.
        /// <br/> If all arguments are null, the text will be added with default characteristics
        /// </summary>
        /// <param name="text">The text to append</param>
        /// <param name="foregroundColor">The color of the text. If null then the color will not change</param>
        /// <param name="backgroundColor">The color of the background of the text. If null, then the color will not change</param>
        /// <param name="formats">Any formats or styles to affect the text</param>
        public AnsiBuilder AppendLine(string text = "", AnsiForeground? foregroundColor = null, AnsiBackground? backgroundColor = null, params AnsiFormat[] formats)
        {
            if (foregroundColor is null && backgroundColor is null && formats.Length == 0)
            {
                Reset();
                stringBuilder.AppendLine(text);
                return this;
            }

            string format = string.Concat(formats.Select(f => f.Code + ";"));

            string forecroundCode = foregroundColor != null ? foregroundColor.Code + ";" : "";
            string backgroundCode = backgroundColor != null ? backgroundColor.Code + ";" : "";

            string codeWithSemicolonEnd = PREFIX + format + backgroundCode + forecroundCode;
            string formattedCode = codeWithSemicolonEnd[..^1] + "m";

            stringBuilder.Append(formattedCode).AppendLine(text);

            return this;
        }

        /// <summary>
        /// Resets the colors and styles from previous lines to default values
        /// </summary>
        public AnsiBuilder Reset()
        {
            stringBuilder.Append("\u001b[0m");
            return this;
        }

        /// <summary>
        /// Converts this builder into a complete body of text with the added codes and the correct format to be 
        /// recognized by Discord
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"```ansi\n{stringBuilder}\n```";

    }
}
