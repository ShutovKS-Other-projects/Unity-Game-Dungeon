//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.UI.Impl.UnityEditor;

namespace DungeonArchitect.Editors.LaunchPad
{
    public class LaunchPadWindow : EditorWindow
    {
        [MenuItem("Dungeon Architect/Launch Pad", priority = 1000)]
        public static void OpenWindow_LaunchPad()
        {
            LaunchPadWindow.OpenWindow();
        }

        class BreadCrumbUserData
        {
            public BreadCrumbUserData(ScreenPage page)
            {
                this.page = page;
                this.scrollPos = Vector2.zero;
            }

            public ScreenPage page;
            public Vector2 scrollPos = Vector2.zero;
        }

        UISystem uiSystem;
        UIRenderer renderer;
        Color backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        BreadCrumbWidget breadCrumb;
        PanelWidget pageHost;
        ScrollPanelWidget pageScroller;
        ListViewWidget<LaunchPadCategoryData> categoryListView;

        public static void OpenWindow()
        {
            LaunchPadWindow window = EditorWindow.GetWindow(typeof(LaunchPadWindow)) as LaunchPadWindow;
            window.Init();
            window.Show(); 
        }

        public void Init()
        {
            titleContent = new GUIContent("Dungeon Architect - Launch Pad");
            CreateUISystem();
        }

        void CreateUISystem()
        {
            uiSystem = new UnityEditorUISystem();
            renderer = new UnityEditorUIRenderer();
            BuildLayout();
        }

        [System.Serializable]
        class SidebarDataEntry
        {
            public string page = "";
            public string title = "";
        }

        [System.Serializable]
        class SidebarData
        {
            public SidebarDataEntry[] items = new SidebarDataEntry[0];
        }

        IWidget CreateReviewWidget()
        {
            var color = 0.5f;
            var link = new LinkWidget(
                new LabelWidget("Leave a review")
                    .SetColor(new Color(color, color, color))
                    .SetFontSize(14)
                    .SetTextAlign(TextAnchor.MiddleCenter))
                .SetDrawLinkOutline(false);

            link.LinkClicked += LeaveReviewLinkClicked;

            return link;
        }

        private void LeaveReviewLinkClicked(WidgetClickEvent clickEvent)
        {
            string assetStoreURL = "https://u3d.as/nAL";
            Application.OpenURL(assetStoreURL);
        }

        void OnInspectorUpdate()
        {
            if (categoryDataSource != null && categoryDataSource.Count == 0)
            {
                LoadCategories();
                if (categoryDataSource.Count > 0)
                {
                    categoryListView.Bind(categoryDataSource);
                    
                    var categories = categoryDataSource.GetItems();
                    if (categories != null && categories.Length > 0)
                    {
                        ShowRootPage(categories[0].path);
                        requestRepaint = true;
                    }
                }
            }
        }

        private LaunchPadCategoryDataSource categoryDataSource = new LaunchPadCategoryDataSource();
        
        void LoadCategories()
        {
            var categories = new List<LaunchPadCategoryData>();
            var datafilePath = "LaunchPad/sidebar";
            var sidebarDataAsset = Resources.Load<TextAsset>(datafilePath);
            if (sidebarDataAsset != null)
            {
                var sidebarData = JsonUtility.FromJson<SidebarData>(sidebarDataAsset.text);
                if (sidebarData != null)
                {
                    foreach (var item in sidebarData.items)
                    {
                        categories.Add(new LaunchPadCategoryData(item.page, item.title));
                    }
                }
            }
            categoryDataSource.SetItems(categories.ToArray());
        }
        
        IWidget BuildCategoriesListView()
        {
            LoadCategories();

            categoryListView = new ListViewWidget<LaunchPadCategoryData>();
            categoryListView.ItemHeight = 45;
            categoryListView.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            categoryListView.Bind(categoryDataSource);
            categoryListView.ItemClicked += OnCategoryItemClicked;
            categoryListView.SelectionChanged += OnCategoryItemClicked;


            var reviewWidget = CreateReviewWidget();

            IWidget content = categoryListView;

            // Add asset store award widget
            {
                // Add asset store award logo
                var awardImage = ScreenPageLoader.LoadImageAsset("misc/UnityAwards16_seal-AssetStore_small");
                var awardImageColor = new Color(1, 1, 1, 0.5f);
                var listViewSize = categoryListView.GetDesiredSize(Vector2.zero, uiSystem);
                var heightOffset = listViewSize.y;

                content = new OverlayPanelWidget()
                    .AddWidget(content)
                    .AddWidget(
                        new StackPanelWidget(StackPanelOrientation.Horizontal)
                        .AddWidget(new NullWidget())
                        .AddWidget(
                            new StackPanelWidget(StackPanelOrientation.Vertical)
                            .AddWidget(new NullWidget(), heightOffset)
                            .AddWidget(
                                new BorderWidget(
                                    new ImageWidget(awardImage)
                                    .SetTint(awardImageColor)
                                    .SetDrawMode(ImageWidgetDrawMode.Fixed))
                                .SetTransparent()
                                .SetPadding(0, 10, 0, 0)
                            , 0, true)
                            .AddWidget(new NullWidget())
                            .AddWidget(reviewWidget, 24)
                        , 0, true)
                        .AddWidget(new NullWidget()));
            }
            return content;
        }

        private void OnCategoryItemClicked(LaunchPadCategoryData item)
        {
            ShowRootPage(item.path);
        }

        IWidget BuildContentWidget()
        {
            breadCrumb = new BreadCrumbWidget();
            breadCrumb.LinkClicked += BreadCrumb_LinkClicked;
            breadCrumb.FontSize = 14;
            breadCrumb.TextColor = new Color(0.5f, 0.5f, 0.5f);

            pageHost = new PanelWidget(new NullWidget());
            pageScroller = new ScrollPanelWidget(pageHost);
            var body = new BorderWidget(pageScroller)
                .SetColor(new Color(0.2f, 0.2f, 0.2f))
                .SetBorderColor(new Color(0, 0, 0, 0))
                .SetPadding(0, 0, 0, 0);

            var content = new StackPanelWidget(StackPanelOrientation.Vertical)
                .AddWidget(
                    new BorderWidget(
                        new BorderWidget(breadCrumb)
                            .SetPadding(5, 5, 5, 5)
                            .SetBorderColor(new Color(0.1f, 0.1f, 0.1f))
                            .SetColor(new Color(0.15f, 0.15f, 0.15f)))
                        .SetPadding(10, 10, 10, 10)
                        .SetBorderColor(new Color(0, 0, 0, 0))
                        .SetColor(new Color(0.2f, 0.2f, 0.2f))
                    , 0, true)
                .AddWidget(body);

            return content;
        }

        void PushPage(string path)
        {
            var page = ScreenPageLoader.LoadPage(path);
            if (page != null)
            {
                var scrollPos = (pageScroller != null) ? pageScroller.ScrollPosition : Vector2.zero;
                var topItemData = breadCrumb.GetTopItemUserData() as BreadCrumbUserData;
                topItemData.scrollPos = scrollPos;

                breadCrumb.PushPage(page.Title, new BreadCrumbUserData(page));

                pageHost.SetContent(page.Widget);
            }
        }

        void ShowRootPage(string path)
        {
            var page = ScreenPageLoader.LoadPage(path);
            if (page != null)
            {
                page.LinkClicked += OnPageLinkClicked;
                pageHost.SetContent(page.Widget);
                breadCrumb.Clear();
                breadCrumb.PushPage(page.Title, new BreadCrumbUserData(page));
            }
            else
            {
                pageHost.SetContent(new NullWidget());
                breadCrumb.Clear();
            }
        }

        private void OnPageLinkClicked(string path)
        {
            PushPage(path);
        }

        private void BreadCrumb_LinkClicked(object userdata)
        {
            var navData = userdata as BreadCrumbUserData;
            if (navData != null && navData.page != null)
            {
                pageHost.SetContent(navData.page.Widget);
                if (pageScroller != null)
                {
                    pageScroller.ScrollPosition = navData.scrollPos;
                }
            }
        }

        IWidget CreateBrandingWidget()
        {
            string label = "DUNGEON ARCHITECT - LAUNCH PAD";
            var padding = 4;
            var branding = new BorderWidget(
                new LabelWidget(label)
                    .SetFontSize(14)
                    .SetTextAlign(TextAnchor.MiddleCenter)
                    .SetColor(new Color(0.75f, 0.75f, 0.75f)))
                .SetPadding(padding, padding, padding, 0)
                .SetColor(new Color(0.1f, 0.1f, 0.1f))
                .SetBorderColor(new Color(0, 0, 0, 0));

            return branding;
        }

        void BuildLayout()
        {
            var categories = BuildCategoriesListView();
            var content = BuildContentWidget();

            categoryListView.SetSelectedIndex(0);

            IWidget layout =
                new BorderWidget()
                .SetColor(backgroundColor)
                .SetBorderColor(new Color(0, 0, 0, 0))
                .SetPadding(5, 5, 6, 6)
                .SetContent(
                    new StackPanelWidget(StackPanelOrientation.Horizontal)
                        .AddWidget(categories, 200)
                        .AddWidget(content));

            layout = new StackPanelWidget(StackPanelOrientation.Vertical)
                .AddWidget(CreateBrandingWidget(), 0, true)
                .AddWidget(layout);

            uiSystem.SetLayout(layout);
        }
        
        private bool requestRepaint = false;

        void Update()
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }
            
            
            if (requestRepaint)
            {
                Repaint();
                requestRepaint = false;
            }
            
        }
        
        private void OnGUI()
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            if (uiSystem.Layout == null)
            {
                BuildLayout();
            }

            var bounds = new Rect(Vector2.zero, position.size);
            uiSystem.Update(bounds);
            
            uiSystem.Draw(renderer);
            
            var e = Event.current;
            if (e.isScrollWheel)
            {
                requestRepaint = true;
            }

            switch (e.type)
            {
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                    requestRepaint = true;
                    break;
            }
            
            HandleInput(e);
        }

        void HandleInput(Event e)
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            if (uiSystem != null && uiSystem.Layout != null)
            {
                var layout = uiSystem.Layout;
                if (e.type == EventType.MouseDown || e.type == EventType.ScrollWheel)
                {
                    WidgetUtils.ProcessInputFocus(e.mousePosition, uiSystem, layout);
                }

                if (uiSystem.IsDragDrop)
                {
                    WidgetUtils.ProcessDragOperation(e, layout, uiSystem);
                }

                UpdateDragDropState(e);

                WidgetUtils.HandleWidgetInput(uiSystem, e, e.mousePosition, layout);
            }

        }

        void UpdateDragDropState(Event e)
        {
            if (uiSystem != null)
            {
                if (e.type == EventType.DragUpdated)
                {
                    uiSystem.SetDragging(true);
                }
                else if (e.type == EventType.DragPerform || e.type == EventType.DragExited)
                {
                    uiSystem.SetDragging(false);
                }
            }
        }
    }

    public class LaunchPadWindowImportLauncher : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool launchWindow = false;
            foreach (var asset in importedAssets)
            {
                if (asset.Contains("DungeonArchitect_LaunchPad"))
                {
                    if (asset.Contains("sidebar.json") || asset.Contains("builder_templates.json"))
                    {
                        launchWindow = true;
                        break;
                    }
                }
            }

            if (launchWindow)
            {
                LaunchPadWindow.OpenWindow();
            }
        }
    }
}
