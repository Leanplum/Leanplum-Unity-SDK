using System;
using System.Collections.Generic;

namespace CleverTapSDK
{
    public class CleverTapInboxMessage
    {
        #region Inbox Message properties
        /// <summary>
        /// Returns the message identifier of the inbox message.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The inbox "msg" Message data.
        /// </summary>
        public MessageData Message { get; set; }

        /// <summary>
        /// Returns true if the inbox message is read.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Returns the delivery timestamp of the inbox message.
        /// </summary>
        public long DateTs { get; set; }

        /// <summary>
        /// Returns the delivery UTC date of the inbox message.
        /// </summary>
        public DateTime? DateUtcDate => DateTs > 0 ? DateTimeOffset.FromUnixTimeSeconds(DateTs).UtcDateTime : null;

        /// <summary>
        /// Returns the expiry timestamp (time to live) of the inbox message or
        /// 0 if time to live is set to infinite.
        /// </summary>
        public long ExpiresTs { get; set; }

        /// <summary>
        /// Returns the expiry UTC date of the inbox message or null if no expiry.
        /// </summary>
        public DateTime? ExpiresUtcDate => ExpiresTs > 0 ? DateTimeOffset.FromUnixTimeSeconds(ExpiresTs).UtcDateTime : null;

        /// <summary>
        /// Returns the campaign identifier.
        /// </summary>
        public string CampaignId { get; set; }

        #endregion

        #region Inbox Message nested classes
        public class MessageData
        {
            /// <summary>
            /// The inbox message type. Possible values are "simple",
            /// "message-icon", "carousel", "carousel-image".
            /// </summary>
            public string MessageType { get; set; }

            /// <summary>
            /// Returns true if tags are set in the inbox message.
            /// </summary>
            public bool EnableTags { get; set; }

            /// <summary>
            /// The inbox content items.
            /// For Simple Message and Icon Message templates the size of this List is by default 1.
            /// For Carousel templates, the size of the List is the number of slides in the Carousel.
            /// </summary>
            public List<Content> Content { get; set; }

            /// <summary>
            /// The inbox style Background Color.
            /// </summary>
            public string BackgroundColor { get; set; }

            /// <summary>
            /// The inbox message orientation.
            /// Returns "l" for landscape.
            /// Returns "p" for portrait.
            /// </summary>
            public string Orientation { get; set; }

            /// <summary>
            /// The inbox message Filter tags.
            /// </summary>
            public List<string> Tags { get; set; }

            /// <summary>
            /// The inbox message Custom Key-Value Pairs.
            /// </summary>
            public List<CustomKV> CustomKeyValuePairs { get; set; }
        }

        /// <summary>
        /// The inbox message Content data.
        /// </summary>
        public class Content
        {
            public long Key { get; set; }

            /// <summary>
            /// The inbox Message text and color.
            /// </summary>
            public TextDetails Message { get; set; }

            /// <summary>
            /// The inbox Title text and color.
            /// </summary>
            public TextDetails Title { get; set; }

            /// <summary>
            /// The inbox message Call to Action and Links.
            /// </summary>
            public ActionDetails Action { get; set; }

            /// <summary>
            /// The inbox message main Image media data.
            /// </summary>
            public Media Media { get; set; }

            /// <summary>
            /// The Inbox message Icon media data.
            /// </summary>
            public Media Icon { get; set; }
        }

        /// <summary>
        /// The inbox message Actions.
        /// </summary>
        public class ActionDetails
        {
            public bool HasUrl { get; set; }
            public bool HasLinks { get; set; }

            /// <summary>
            /// The On Message Call to Action Url.
            /// </summary>
            public Url Url { get; set; }

            /// <summary>
            /// The inbox message On Link links.
            /// </summary>
            public List<Link> Links { get; set; }
        }

        /// <summary>
        /// The inbox message Link.
        /// </summary>
        public class Link
        {
            /// <summary>
            /// The Link type. Values could be "copy", "url", "kv".
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// The Link text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// The Link Text color.
            /// </summary>
            public string Color { get; set; }

            /// <summary>
            /// The Link Fill color.
            /// </summary>
            public string BackgroundColor { get; set; }

            /// <summary>
            /// The Copy to clipboard text value for "copy" type links.
            /// </summary>
            public TextValue CopyText { get; set; }

            /// <summary>
            /// The Link URL for "Open URL" links.
            /// </summary>
            public Url Url { get; set; }

            /// <summary>
            /// The Link Custom Key-Value pairs.
            /// </summary>
            public Dictionary<string, string> KeyValuePairs { get; set; }
        }

        /// <summary>
        /// Text fields Text and Color.
        /// </summary>
        public class TextDetails
        {
            public string Text { get; set; }
            public string Color { get; set; }
        }

        /// <summary>
        /// The URLs for iOS and Android.
        /// </summary>
        public class Url
        {
            public TextValue Android { get; set; }
            public TextValue IOS { get; set; }
        }

        /// <summary>
        /// The Media data including URL and content type of the asset.
        /// </summary>
        public class Media
        {
            public string Url { get; set; }
            public string Key { get; set; }
            public string ContentType { get; set; }
        }

        /// <summary>
        /// Custom Key-Value Pair.
        /// </summary>
        public class CustomKV
        {
            public string Key { get; set; }
            public TextValue Value { get; set; }
        }

        /// <summary>
        /// The objects "text" value representation. 
        /// </summary>
        public class TextValue
        {
            public string Text { get; set; }
        }
        #endregion
    }
}