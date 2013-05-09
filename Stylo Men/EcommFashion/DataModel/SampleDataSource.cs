using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace EcommFashion.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : EcommFashion.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Nivax Data");

            var group1 = new SampleDataGroup("Group-1",
                    "Dress Shirt Styles",
                    "Group Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group1.Items.Add(new SampleDataItem("Group-1-Item-1",
                    "White, semi-spread, light to mid-weight Poplin/Pinpoint/Twill",
                    "",
                    "Assets/HubPage/HubPage1.png",
                    "Quick, think of a white dress shirt.  Yahtzee.  There’s the #1 shirt we should all have in our closet.  The collar does not button down on these, and the fabric is a mid to lightweight (we’ll get to the thicker oxford cloth soon).  Some call these oxfords even though that’s not quite right.  It’s a shirt that’ll look equally as good with a suit as under a v-neck with jeans.  It’s crisp, it’s clean, and you want a collar with enough beef  to look substantial.  Use some wurkin’ stiffs to keep it framing your face when tieless.  Melt your brain with the differences between poplin, pinpoint, & twill here, here, and here (scroll down a bit on that last one).",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-2",
                   "Light Blue, semi-spread, light to mid-weight Poplin/Pinpoint/Twill",
                    "",
                    "Assets/HubPage/HubPage2.png",
                    "Pretty much the same thing as the white dress shirt at #1, only in a real light blue color.  Not royal blue, not kinda light blue with grey buttons, light blue with standard off-white/bone colored buttons.  Slightly less formal than white, but still able to be dressed way up.  The BR shirt above doesn’t come in neck-and-sleeve measurements, but it can get cheap when on sale.  It’s also been quite durable on this end, after a ton of beatings in the washer & dryer.",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-3",
                    " White, mid-weight to heavy-weight Oxford Cloth Button Down",
                    "",
                    "Assets/HubPage/HubPage3.png",
                    "The OCBD.  Which surprisingly, wasn’t a member of the WuTang clan.  OCBD = Oxford Cloth Button Down.  And the “button down” part refers to the collar, not the fact that you button the shirt in front.  The term for that would be “button up” (even, if like me, you start at the top and button down.  Let’s move on.)  Is it a “dress” shirt?  That can really depend on how thick the fabric is.  If it’s like the J. Crew Factory above, or maybe the options from Old Navy, Bonobos or LEC, the thickness and rumpled-ness (not a word) of the cloth will make it hard to dress up.  But lighter, pressed OCBDs can pull some duty at the office.  A workhorse for layering.  Think grey jeans with a blue cotton blazer.",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-4",
                    "White base, blue windowpane / tattersall, non-button-down",
                    "",
                    "Assets/HubPage/HubPage4.png",
                    "Lots of white and blue right?  Well, yes.  An orange and green check just won’t go with as much stuff in your closet.  Meanwhile, a windowpane or tattersall is just different enough from the usual striped shirts most guys default to.  When under a jacket of some kind, they’ll give your look a bit of depth.  And unlike gingham (we’re getting there) they’ve got plenty of white which is more business ready.  When it comes to the office, for paterns, Tic-Tac-Toe > Checkers.  And for the size of the squares, the ideal size is between a pencil eraser and a quarter.",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-5",
                    "The Bold Gingham non-button-down",
                    "",
                    "Assets/HubPage/HubPage5.png",
                    "Color is up to you.  Black and white obviously offers the most contrast, but deep blue, red, even purple can deliver.  It’s a dressed up version of a dressed down pattern.",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-6",
                    " White base, thin stripe non-button-down",
                    "",
                    "Assets/HubPage/HubPage6.png",
                    "The stripes on these are thin enough that white base is dominant, but the stripes add a bit of depth and maybe some more color.  Thicker striped ties look just fine on top.  Just keep the thickness of the patterns far enough apart.  Keep your stripes slim and you can even take some color risks, like the tan stripe on the pictured cotton-blend, cheap Alfani Red.",
                    ITEM_CONTENT,
                    group1));
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                    "t-Shirt Styles",
                    "Group Subtitle: 2",
                    "Assets/LightGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group2.Items.Add(new SampleDataItem("Group-2-Item-1",
					"Last Exit To Nowhere",
					"",
                    "Assets/HubPage/HubPage7.png",
                    "You didn't have to spend your childhood Betamaxed out on TRON for certain classic movies to become a part of your life. Celebrate your celluloid intimacy like a true connoisseur with a Last Exit to Nowhere T-shirt.These high-quality, short-sleeve crew necks pull logos of iconic companies and places from seriously canonic flicks, offering a wink to fellow devotees without trumpeting embarrassing fanboydom to people who have no idea just how well a rug can tie a room",
                    ITEM_CONTENT,
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-2",
                    "Hendrix Shirts",
                    "",
                    "Assets/HubPage/HubPage8.png",
                    "A collaboration between Hendrix's sister and Altamont cofounder Andrew Reynolds, the line's emblazoned with vintage backstage art from the man who once famously observed my other-worldly guitar skills can make up for my unintelligible singing. Each garment begins as a soft, 100% cotton, ring-spun T-shirt, then gets printed with Jimi-created designs, like the Rooster (a purple haze'd cock) and the Bride (a beautiful woman stretched out on the white field of your distended belly).",
                    ITEM_CONTENT,
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-3",
                     "Beard And Bangs",
                    "",
                    "Assets/HubPage/HubPage9.png",
                    "Many graphic T-shirts are inspired by drugs you've rarely taken -- so why not change things up with shirts inspired by history you've scarcely read? Flaunt ye olde school, with Beard and Bangs. Handmade in Brooklyn, BB's 100% cotton shirts are designed along historical themes, with the current collection taken from arcane postage stamps and a line from a Dylan song (Tombstone Blues: confounding fans with oblique references since 1965).",
                    ITEM_CONTENT,
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-4",
                    "LOLA",
                   "",
                   "Assets/HubPage/HubPage10.png",
                   "Like a tiny island nation that can only export phosphates, many smaller designers exhibit a range that's severely limited by their capability. For a small company that's going in so many directions that it no longer knows where it is, check out LOLA. Based out of New York, LOLA's theme is themelessness: A happily disconnected line of polos, hoodies and T-shirts influenced by such iconic touchstones as seafaring, gangsta rap and youth sports.",
                   ITEM_CONTENT,
                   group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-5",
                    "Hard Rider NYC",
                   "",
                   "Assets/HubPage/HubPage11.png",
                   "Tattoos are man's oldest and boldest expression of creativity. Unfortunately, they're also man's oldest and boldest source of hepatitis C. Exercise that same expressiveness on your clothes with Hard Rider NYC.",
                   ITEM_CONTENT,
                   group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-6",
                    "Zachary Prell",
                   "",
                   "Assets/HubPage/HubPage12.png",
                   "It's bad enough that you're forced to wear button downs to work, but adding insult to injury are their annoyingly unnecessary structural defects -- it's like sitting in an electric chair that also smells bad. For an obsessively achieved solution, try Zachary Prell. ZP was founded by an ex-Wall Streeter who spent three years creating his ultimate office shirt (including one year scouring mills around the world for fabric soft enough for hard-working men). The result is a shirt that’s free of the odd button spacing, bunching, and other imperfections that can make wearing even a high-end label a nagging hell.",
                   ITEM_CONTENT,
                   group2));
            this.AllGroups.Add(group2); 


        }
    }
}
