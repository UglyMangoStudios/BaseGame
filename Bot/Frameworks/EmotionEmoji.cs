namespace SpaceDiscordBot.Frameworks
{
    /// <summary>
    /// An enum-like class that contains potential emojis to express certain emojis or situations.
    /// <br/> Note: these are default/vanilla emojis that are used globally
    /// </summary>
    internal class EmotionEmoji
    {
        public static readonly EmotionEmoji Cowboy = new(":cowboy:");
        public static readonly EmotionEmoji Scream = new(":scream:");
        public static readonly EmotionEmoji Fear = new(":fearful:");
        public static readonly EmotionEmoji Goofy = new(":zany_face:");
        public static readonly EmotionEmoji Starstruck = new(":star_struck:");
        public static readonly EmotionEmoji Smile = new(":smile:");
        public static readonly EmotionEmoji HoldOn = new(":point_up:");
        public static readonly EmotionEmoji SweatSmile = new(":sweat_smile:");
        public static readonly EmotionEmoji Laughing = new(":joy:");
        public static readonly EmotionEmoji ROFL = new(":rofl:");
        public static readonly EmotionEmoji Naughty = new(":smiling_imp:");
        public static readonly EmotionEmoji MindBlown = new(":exploding_head:");
        public static readonly EmotionEmoji SmallCri = new(":cry:");
        public static readonly EmotionEmoji BigCri = new(":sob:");
        public static readonly EmotionEmoji Kiss = new(":kissing_heart:");
        public static readonly EmotionEmoji Cool = new(":sunglasses:");
        public static readonly EmotionEmoji Party = new(":partying_face:");
        public static readonly EmotionEmoji Confounded = new(":confounded:");
        public static readonly EmotionEmoji Dead = new(":dizzy_face");
        public static readonly EmotionEmoji Forgor = new(":skull:");
        public static readonly EmotionEmoji Strong = new(":muscle:");
        public static readonly EmotionEmoji Wave = new(":wave:");
        public static readonly EmotionEmoji SideEye = new(":eyes:");
        public static readonly EmotionEmoji SimpleLove = new(":heart:", (255, 0, 0));
        public static readonly EmotionEmoji SwingingLove = new(":revolving_hearts:");
        public static readonly EmotionEmoji FireLove = new(":heart_on_fire:");
        public static readonly EmotionEmoji Sparkle = new(":sparkles:");
        public static readonly EmotionEmoji Star = new(":star:");
        public static readonly EmotionEmoji Fire = new(":fire:");
        public static readonly EmotionEmoji Flower = new(":cherry_blossom:");
        public static readonly EmotionEmoji Lightning = new(":lightning:");
        public static readonly EmotionEmoji Spark = new(":boom:");
        public static readonly EmotionEmoji SusSplash = new(":sweat_drops:");
        public static readonly EmotionEmoji SusFruit = new(":peach:");
        public static readonly EmotionEmoji SusVeggie = new(":eggplant:");
        public static readonly EmotionEmoji Amazing100 = new(":100:");
        public static readonly EmotionEmoji Success = new(":white_check_mark:");
        public static readonly EmotionEmoji Failure = new(":x:");
        public static readonly EmotionEmoji Rock = new(":moyai:");
        public static readonly EmotionEmoji SleepyHead = new(":sleeping:");
        public static readonly EmotionEmoji Ring = new(":bell:");
        public static readonly EmotionEmoji StrongAnnouncement = new(":loudspeaker: ");
        public static readonly EmotionEmoji SoftAnnouncement = new(":mega:");


        /// <summary>
        /// The Id of the emoji that is used within discord
        /// </summary>
        public string DiscordId { get; private set; }

        /// <summary>
        /// The color this emoji represents. This is used when coloring the embed
        /// </summary>
        public (byte r, byte g, byte b) Color { get; private set; }

        /// <summary>
        /// Internally create an emoji object
        /// </summary>
        /// <param name="discordId">The id of the emoji within discord</param>
        /// <param name="color">The color of the emotion emoji. If null, a default emoji yellow will be used</param>
        private EmotionEmoji(string discordId, (byte r, byte g, byte b)? color = null)
        {
            DiscordId = discordId;
            Color = color ?? (255, 222, 52);
        }

        /// <summary>
        /// Converts this object into the id of the emoji
        /// </summary>
        /// <returns></returns>
        public override string ToString() => DiscordId;

        /// <summary>
        /// Implicit conversion of this emoji into a string
        /// </summary>
        /// <param name="emoji"></param>

        public static implicit operator string(EmotionEmoji emoji) => emoji.ToString();
    }
}
