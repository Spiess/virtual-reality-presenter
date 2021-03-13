# virtual-reality-presenter
Unity package containing assets and scripts for presentations in virtual reality.

This package uses Unity Layer 30 for all presentation UI which should not be rendered by the presentation camera.
If you are using layer 30 in your project, please adjust the prefabs from this package accordingly.
If you are using other non-standard layers containing objects that should be rendered, please add these to the presentation camera render mask.
It is recommended that you rename layer 30 to "Presentation UI" if it wasn't in use before.
