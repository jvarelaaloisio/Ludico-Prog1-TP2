# UnityPackage-Core
TO DO
<!--A simple extension on Unity's prefs system that allows for easier control and leads to cleaner code.
## How to use
A pref unit is an object that contains a value and it's corresponding key (Basically, a key-value pair object) that interfaces with a pref database
This pref is injected in construction with:
  - *Prefs* ([IPrefs](https://github.com/jvarelaaloisio/UnityPackage-PrefUnits#reasoning-behind-the-iprefs-interface)): An object implementing IPrefs, which functions as an interface to either PlayerPrefs, EditorPrefs, or any other prefs library that the user wishes to implement.
  - *Key* (string): The key that will be used to get and set the values on Load/Save.
  - *Default Value* (T): The value to set when first constructing the object and before loading/trying to load the corresponding value.
## Out of the box implementations
### Pref Units
The package comes with a couple of pref unit implementations and, to add any other custom implementation, the only thing needed is to inherit from `PrefUnit<T>` where *T* is the type of the stored value.
### EditorPrefsWrapper
A simple wrapper that maps all methods to the corresponding ones found in the EditorPrefs static class.
### PlayerPrefsAdapter
An adapter that allows for all pref units to be saved into player prefs. Only difference to EditorPrefsWrapper is that PlayerPrefs doesn't allow for boolean values, so those are stored as an integer (1 for true, 0 for false).
## Reasoning behind the IPrefs interface
Basically, PlayerPrefs and EditorPrefs don't share a common interface, which led me to implement the adapter pattern so to get an abstract version of both classes.-->
