// Copyright 2021 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using System;
using UIKit;

namespace ArcGISRuntime.Samples.OpenMapURL
{
    [Register("OpenMapURL")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        name: "Open map URL",
        category: "Map",
        description: "Display a web map.",
        instructions: "A web map can be selected from the drop-down list. On selection the web map displays in the map view.",
        tags: new[] { "portal item", "web map" })]
    public class OpenMapURL : UIViewController
    {
        // Hold references to UI controls.
        private MapView _myMapView;
        private UIBarButtonItem _selectMapButton;

        // String array to hold urls to publicly available web maps.
        private string[] _itemURLs = {
            "https://arcgis.com/home/item.html?id=92ad152b9da94dee89b9e387dfe21acd",
            "https://arcgis.com/home/item.html?id=5be0bc3ee36c4e058f7b3cebc21c74e6",
            "https://arcgis.com/home/item.html?id=064f2e898b094a17b84e4a4cd5e5f549"
        };

        // String array to store titles for the webmaps specified above.
        private string[] _titles = {
            "Geology for United States",
            "Terrestrial Ecosystems of the World",
            "Recent Hurricanes, Cyclones and Typhoons"
        };

        public OpenMapURL()
        {
            Title = "Open map (URL)";
        }

        private void Initialize()
        {
            // Show the first webmap by default.
            _myMapView.Map = new Map(new Uri(_itemURLs[0]));
        }

        private void OnMapsButtonTouch(object sender, EventArgs e)
        {
            // Initialize an UIAlertController with a title and style of an alert.
            UIAlertController actionSheetAlert = UIAlertController.Create("Select a map to open", "", UIAlertControllerStyle.Alert);

            // Add actions to alert. Selecting an option re-initializes the Map
            // with selected webmap URL and assigns it to MapView.
            actionSheetAlert.AddAction(UIAlertAction.Create(_titles[0], UIAlertActionStyle.Default,
                action => _myMapView.Map = new Map(new Uri(_itemURLs[0]))));
            actionSheetAlert.AddAction(UIAlertAction.Create(_titles[1], UIAlertActionStyle.Default,
                action => _myMapView.Map = new Map(new Uri(_itemURLs[1]))));
            actionSheetAlert.AddAction(UIAlertAction.Create(_titles[2], UIAlertActionStyle.Default,
                action => _myMapView.Map = new Map(new Uri(_itemURLs[2]))));
            PresentViewController(actionSheetAlert, true, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Initialize();
        }

        public override void LoadView()
        {
            // Create the views.
            View = new UIView { BackgroundColor = ApplicationTheme.BackgroundColor };

            _myMapView = new MapView();
            _myMapView.TranslatesAutoresizingMaskIntoConstraints = false;

            _selectMapButton = new UIBarButtonItem();
            _selectMapButton.Title = "Select a map";

            UIToolbar toolbar = new UIToolbar();
            toolbar.TranslatesAutoresizingMaskIntoConstraints = false;
            toolbar.Items = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                _selectMapButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            };

            // Add the views.
            View.AddSubviews(_myMapView, toolbar);

            // Lay out the views.
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                _myMapView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
                _myMapView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                _myMapView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
                _myMapView.BottomAnchor.ConstraintEqualTo(toolbar.TopAnchor),

                toolbar.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor),
                toolbar.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                toolbar.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor)
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Subscribe to events.
            _selectMapButton.Clicked += OnMapsButtonTouch;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            // Unsubscribe from events, per best practice.
            _selectMapButton.Clicked -= OnMapsButtonTouch;
        }
    }
}