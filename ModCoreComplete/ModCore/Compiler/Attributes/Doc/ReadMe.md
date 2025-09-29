=== ModInfoAttribute ===

Summary:
The ModInfoAttribute is used to provide basic metadata about a mod, such as its ID, author, and version.
While not strictly required for a mod to load, it is strongly recommended. Certain systems—such as mod
dependency resolution—require this attribute to function correctly.

Usage Example:
[ModInfo("ExampleMod", "YourName", "1.0.0")]
public class ExampleMod : MonoBehaviour
{
    // Your mod implementation
}

Constructor Parameters:
- modID: A unique ID used to reference this mod (e.g., "ExampleMod").
- author: The name of the mod creator.
- version: The version string (e.g., "1.0.0").

Important Notes:
- This attribute is not required for a mod to load, but is strongly recommended.
- Mods that use ModDependencyAttribute will fail to load if the mod they depend on
  does not have a ModInfoAttribute.
- The loader uses modID from ModInfoAttribute to resolve dependencies.

=== ModDependencyAttribute ===

Summary:
The ModDependencyAttribute is used to declare that a mod depends on another mod.
This ensures that the referenced mod is loaded before the dependent one.

Usage Example:
[ModDependency("ExampleMod")]
public class ExampleMod : MonoBehaviour
{
    // Your mod implementation
}

Constructor Parameters:
- modID: The ID of the mod this mod depends on. This must match the modID defined
  in the target mod’s ModInfoAttribute.

📝 Important Notes:
You can apply ModDependencyAttribute multiple times to declare multiple dependencies.
If a referenced mod does not define a ModInfoAttribute, the loader cannot resolve the dependency and will skip loading the dependent mod.
Always ensure that all required mods define a unique modID using ModInfoAttribute.

⚠️ Warning:
Avoid using ModDependencyAttribute to reference an IMod mod from a MonoBehaviour mod (or vice versa). This can lead to loading errors and various exceptions. Proceed at your own risk.

Dependency Resolution:
- Both loaders support ModDependencyAttribute.
- Neither loader can load a mod that depends on another mod if the target mod does not
  define a valid ModID through ModInfoAttribute.
- Dependencies are resolved using the ModID value, and if it cannot be found, the mod will
  be skipped to avoid errors or undefined behavior.