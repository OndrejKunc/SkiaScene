# SkiaScene
Collection of lightweight libraries which can be used to simplify manuplation with SkiaSharp graphic objects. 

Supported platforms are .NET Standard 1.3, UWP, Xamarin.iOS, Xamarin.Android.

Currently in prerelease.

## Libraries

* SkiaScene
```
Install-Package SkiaScene -PreRelease
```
Implemented as .NET Standard 1.3 library.

Basic class which allows controlling `SKCanvas` transformations without the need to directly manipulate underlaying Matrix.
First you need to implement `Render` method of `ISKSceneRenderer` interface, where you define all your drawing logic on `SKCanvas`.
Then create the `SKScene` instance:

```csharp
//Create scene
var myRenderer = new MyRenderer(); //user defined instance of ISKSceneRenderer 
scene = new SKScene(myRenderer, canvasView.CanvasSize); //Pass canvas size of your canvas view control

//In your paint method
scene.Render(canvas); //Pass canvas from SKPaintSurfaceEventArgs

//Scene manipulation
scene.MoveByVector(new SKPoint(10, 0)); //Move by 10 units to the right independently from current rotation and zoom
scene.ZoomByScaleFactor(scene.GetCenter(), 1.2f); //Zoom to the center
scene.RotateByRadiansDelta(scene.GetCenter(), 0.1f); //Rotate around center
canvasView.InvalidateSurface(); //Force to repaint
```

* TouchTracking
```
Install-Package TouchTracking -PreRelease
```
Implemented as .NET Standard 1.3 and platform specific libraries.

TouchTracking provides unified API for multi-touch gestures on Android, iOS and UWP. It can be used without Xamarin.Forms. 
Basic principles are described in Xamarin Documentation https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/effects/touch-tracking/

Usage is similar on each platform. 

Android example:

```csharp
//Initialization
canvasView = FindViewById<SKCanvasView>(Resource.Id.canvasView); //Get SKCanvasView
touchHandler = new TouchHandler();
touchHandler.RegisterEvents(canvasView); //Pass View to the touch handler
touchHandler.TouchAction += OnTouch; //Listen to the touch gestures

void OnTouch(object sender, TouchActionEventArgs args) {
    var point = args.Location; //Point location
    var type = args.Type; //Entered, Pressed, Moved ... etc.
    //... do something
}
```

* TouchTracking.Forms
```
Install-Package TouchTracking.Forms -PreRelease
```
Implemented as .NET Standard 1.3 and platform specific libraries.

Same functionality as TouchTracking library but can be consumed in Xamarin.Forms as an Effect called TouchEffect.

```
xmlns:tt="clr-namespace:TouchTracking.Forms;assembly=TouchTracking.Forms"

<Grid>
    <views:SKCanvasView x:Name="canvasView" />
    <Grid.Effects>
        <tt:TouchEffect Capture="True" TouchAction="OnTouch" />
    </Grid.Effects>
</Grid>
```

* SkiaScene.TouchManipulations
```
Install-Package SkiaScene.TouchManipulations -PreRelease
```
Implemented as .NET Standard 1.3 library.

Combines SkiaScene and TouchTracking libraries to respond to the touch and pan gestures. Most of the functionality is described in Xamarin Documentation https://developer.xamarin.com/guides/xamarin-forms/advanced/skiasharp/transforms/touch/

`TouchManipulationManager` recieves touch event info in 'ProcessTouchEvent' method and executes correct gesture in underlying `SKScene`.

`TouchManipulationRenderer` is a thin wrapper around `TouchManipulationManager` which controls frequency of calling `InvalidateSurface` method through `MaxFramesPerSecond` property.
