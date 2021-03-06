// Copyright 2019 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using UIKit;
using System;
using Color = System.Drawing.Color;
using ArcGISRuntime;

namespace ArcGISRuntimeXamarin.Samples.SceneSymbols
{
    [Register("SceneSymbols")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        name: "Scene symbols",
        category: "Symbology",
        description: "Show various kinds of 3D symbols in a scene.",
        instructions: "When the scene loads, note the different types of 3D symbols that you can create.",
        tags: new[] { "3D", "cone", "cube", "cylinder", "diamond", "geometry", "graphic", "graphics overlay", "pyramid", "scene", "shape", "sphere", "symbol", "tetrahedron", "tube", "visualization" })]
    public class SceneSymbols : UIViewController
    {
        // Hold references to UI controls.
        private SceneView _mySceneView;

        private readonly string _elevationServiceUrl = "https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer";

        public SceneSymbols()
        {
            Title = "Scene symbols";
        }

        private void Initialize()
        {
            // Configure the scene with an imagery basemap.
            _mySceneView.Scene = new Scene(BasemapStyle.ArcGISImageryStandard);

            // Add a surface to the scene for elevation.
            ArcGISTiledElevationSource elevationSource = new ArcGISTiledElevationSource(new Uri(_elevationServiceUrl));
            Surface elevationSurface = new Surface();
            elevationSurface.ElevationSources.Add(elevationSource);
            _mySceneView.Scene.BaseSurface = elevationSurface;

            // Create the graphics overlay.
            GraphicsOverlay overlay = new GraphicsOverlay();

            // Set the surface placement mode for the overlay.
            overlay.SceneProperties.SurfacePlacement = SurfacePlacement.Absolute;

            // Create a graphic for each symbol type and add it to the scene.
            int index = 0;
            Color[] colors = {Color.Red, Color.Green, Color.Blue, Color.Purple, Color.Turquoise, Color.White};
            Array symbolStyles = Enum.GetValues(typeof(SimpleMarkerSceneSymbolStyle));
            foreach (SimpleMarkerSceneSymbolStyle symbolStyle in symbolStyles)
            {
                // Create the symbol.
                SimpleMarkerSceneSymbol symbol = new SimpleMarkerSceneSymbol(symbolStyle, colors[index], 200, 200, 200, SceneSymbolAnchorPosition.Center);

                // Offset each symbol so that they aren't in the same spot.
                double positionOffset = 0.01 * index;
                MapPoint point = new MapPoint(44.975 + positionOffset, 29, 500, SpatialReferences.Wgs84);

                // Create the graphic from the geometry and the symbol.
                Graphic item = new Graphic(point, symbol);

                // Add the graphic to the overlay.
                overlay.Graphics.Add(item);

                // Increment the index.
                index++;
            }

            // Show the graphics overlay in the scene.
            _mySceneView.GraphicsOverlays.Add(overlay);

            // Set the initial viewpoint.
            Camera initalViewpoint = new Camera(28.9672, 44.9858, 2495, 12, 53, 0);
            _mySceneView.SetViewpointCamera(initalViewpoint);
        }

        public override void LoadView()
        {
            // Create the views.
            View = new UIView() { BackgroundColor = ApplicationTheme.BackgroundColor };

            _mySceneView = new SceneView();
            _mySceneView.TranslatesAutoresizingMaskIntoConstraints = false;

            // Add the views.
            View.AddSubviews(_mySceneView);

            // Lay out the views.
            _mySceneView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            _mySceneView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _mySceneView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
            _mySceneView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Initialize();
        }
    }
}