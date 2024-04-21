using Discord;
using Discord.WebSocket;

namespace SpaceDiscordBot.Frameworks
{
    /// <summary>
    /// An embed helper class that allows for an interactable book where the user cycles through a series of embeds (aka pages)
    /// 
    /// <br/>TODO: Add restrictions of usage to specific users
    /// </summary>
    internal class EmbedBookBuilder
    {
        /// <summary>
        /// A list of all of id/embed book builder pairs for future reference
        /// </summary>
        private static readonly Dictionary<string, EmbedBookBuilder> _builtBooks = new();

        /// <summary>
        /// The entry method when responding to discord events. 
        /// <br/> This method get the information from the event and sends the next page in the book to the interaction
        /// </summary>
        /// <param name="component">The event data object from Discord</param>
        /// <returns>A boolean that represents if the event was managed and handled</returns>
        public static async Task<bool> ManageBookEvent(SocketMessageComponent component)
        {
            string buttonId = component.Data.CustomId;

            string[] splitButtonId = buttonId.Split("..");
            if (splitButtonId.Length != 3) return false;
            if (splitButtonId[0] != "book") return false;

            string bookId = splitButtonId[1];
            if (!_builtBooks.ContainsKey(bookId)) return false;

            EmbedBookBuilder book = _builtBooks[bookId];

            int currentPage = book.CurrentPageIndex;

            currentPage += splitButtonId[2] switch
            {
                "left" => -1,
                "right" => 1,
                _ => 0
            };
            if (currentPage < 0) currentPage = book.PageCount - 1;

            currentPage %= book.PageCount;

            Page page = book.pages[currentPage];
            book.CurrentPageIndex = currentPage;

            await component.UpdateAsync(p =>
            {
                p.Embed = page.Embed;
                p.Attachments = page.Attachments.Select(a => a.AsFileAttachment()).ToArray();
            });

            return true;
        }

        /// <summary>
        /// An ordered list with the pages
        /// </summary>
        private readonly List<Page> pages = new();

        /// <summary>
        /// Property for the number of pages this book contains
        /// </summary>
        private int PageCount => pages.Count;

        /// <summary>
        /// Recording of the current page that the user is currently on
        /// </summary>

        private int CurrentPageIndex = 0;

        /// <summary>
        /// The id of this book
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Generates the full id of this book by utilizing the message id
        /// </summary>
        private string FullId => MessageId + Id;

        /// <summary>
        /// The message id of this interaction
        /// </summary>
        private ulong MessageId = 0;

        /// <summary>
        /// The start of creating this embed book builder.
        /// </summary>
        /// <param name="bookId">The id of this book. Please note it cannot contain ".." otherwise an exception is thrown</param>
        /// <exception cref="ArgumentException">If the id contains ..</exception>
        public EmbedBookBuilder(string bookId)
        {
            if (bookId.Contains(".."))
                throw new ArgumentException("BookId cannot have .. cuz thats a reserved token for this class.");
            Id = bookId;
        }

        /// <summary>
        /// Adds embeds to this book. 
        /// </summary>
        /// <param name="pages">A collection of embeds to add. Note these embeds cannot contain any files otherwise they will not reder</param>
        /// <returns>The object used for method chaining</returns>
        public EmbedBookBuilder WithPages(params Embed[] pages)
        {
            pages.ToList().ForEach(p => WithPage(p));
            return this;
        }

        /// <summary>
        /// Adds an embed to this book with the potential of file attachments
        /// </summary>
        /// <param name="embed">The embed to add</param>
        /// <param name="index">If not null, override the index this page will be placed in.</param>
        /// <param name="pageAttachments">A list of attachment details pertaining to this one page</param>
        /// <returns>This object used for method chaining</returns>
        /// <exception cref="ArgumentException">Thrown if the index is less than 0</exception>
        public EmbedBookBuilder WithPage(Embed embed, int? index = null, params PageAttachment[] pageAttachments)
        {
            Page page = new(embed, pageAttachments);
            if (index is null) pages.Add(page);
            else
            {
                if (index < 0)
                    throw new ArgumentException("index cannot be less than zero");

                pages.Insert((int)index, page);
            }

            return this;
        }

        /// <summary>
        /// Compiles all of the pages and adds this book builder into the list that is used for future reference.
        /// <br/> Also, with the provided interaction, the first page is retrieved and sent to the interaction for display
        /// </summary>
        /// <param name="interaction">The interaction to set the first embed too</param>
        /// <param name="doAutoDefer">If true, this method will handle the starting interaction for you</param>
        /// <param name="ephemeral">If true, all of the embeds will have the ephemeral property</param>
        /// <returns>An await able task to perform the necessary async operations</returns>
        /// <exception cref="InvalidOperationException">Thrown if the interaction does not meet the criteria to have proper responses</exception>
        public async Task BuildToInteraction(SocketInteraction interaction, bool doAutoDefer = true, bool ephemeral = false)
        {
            if (PageCount == 0) return;

            if (!interaction.HasResponded && doAutoDefer)
                await interaction.DeferAsync(ephemeral);
            else if (!interaction.HasResponded)
                throw new InvalidOperationException("This interaction requires you to defer or respond to it " +
                    "before this book embed manager is valid.");

            var message = await interaction.ModifyOriginalResponseAsync(p => { p.Content = "(☝⌛)"; });
            MessageId = message.Id;

            _builtBooks[FullId] = this;

            Page landingPage = pages[0];
            MessageComponent components = BuildBookComponents();

            await interaction.ModifyOriginalResponseAsync(properties =>
            {
                properties.Content = "";
                properties.Embed = landingPage.Embed;
                properties.Components = components;
                properties.Attachments = landingPage.Attachments.Select(a => a.AsFileAttachment()).ToArray();
            });
        }

        /// <summary>
        /// Builds the buttons that will be on the message
        /// </summary>
        /// <returns>The built buttons as a Message Component</returns>
        private MessageComponent BuildBookComponents()
        {
            ComponentBuilder builder = new();

            ButtonBuilder leftButton = new();
            ButtonBuilder rightButton = new();

            leftButton.WithStyle(ButtonStyle.Secondary).WithLabel("◀");
            rightButton.WithStyle(ButtonStyle.Secondary).WithLabel("▶");

            leftButton.WithCustomId("book.." + FullId + "..left");
            rightButton.WithCustomId("book.." + FullId + "..right");

            builder.WithButton(leftButton).WithButton(rightButton);

            return builder.Build();
        }

        /// <summary>
        /// A class that represents the attachments on an embed or message
        /// </summary>
        internal class PageAttachment
        {
            /// <summary>
            /// The actual path to the attachment. For example, the relative path to an image
            /// </summary>
            public readonly string Path;

            /// <summary>
            /// The name of the file
            /// </summary>
            public string? FileName;
            /// <summary>
            /// The description of the file
            /// </summary>
            public string? FileDescription;

            /// <summary>
            /// If true, discord will mark the image with a spoiler
            /// </summary>
            public bool IsSpoiler;

            /// <summary>
            /// Creates the attachment data for a page
            /// </summary>
            /// <param name="path">The actual path to the attachment. For example, the relative path to an image</param>
            /// <param name="fileName">The name of the file</param>
            /// <param name="fileDesc">The description of the file</param>
            /// <param name="isSpoiler">If true, discord will mark the image with a spoiler</param>
            public PageAttachment(string path, string? fileName = null, string? fileDesc = null, bool isSpoiler = false)
            {
                Path = path;
                FileName = fileName;
                FileDescription = fileDesc;
                IsSpoiler = isSpoiler;
            }

            /// <summary>
            /// Converts this class into a data structure used by the Discord API
            /// </summary>
            /// <returns></returns>
            public FileAttachment AsFileAttachment() => new(Path, FileName, FileDescription, IsSpoiler);

            /// <summary>
            /// A simple implicit cast definition to convert a tuple into a <see cref="PageAttachment"/>
            /// </summary>
            /// <param name="t"></param>
            public static implicit operator PageAttachment((string path, string? fileName, string? fileDesc, bool isSpoiler) t)
                => new(t.path, t.fileName, t.fileDesc, t.isSpoiler);

            /// <summary>
            /// An implicit cast that converts the path of an attachment into an attachment object
            /// </summary>
            /// <param name="path">The path of the attachment</param>
            public static implicit operator PageAttachment(string path) => new(path);
        }

        /// <summary>
        /// Data representation of an embed and any page attachments it may contain
        /// </summary>
        private class Page
        {
            /// <summary>
            /// The embed that is this page
            /// </summary>
            public readonly Embed Embed;

            /// <summary>
            /// A collection of all attachments that may exist in this page
            /// </summary>
            public readonly PageAttachment[] Attachments;

            /// <summary>
            /// Creates a page
            /// </summary>
            /// <param name="embed">The embed that is this page</param>
            /// <param name="attachments">A collection of all attachments that may exist in this page</param>
            public Page(Embed embed, params PageAttachment[] attachments)
            {
                Embed = embed;
                Attachments = attachments;
            }


        }

    }
}
