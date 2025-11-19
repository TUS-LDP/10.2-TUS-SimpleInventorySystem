using UnityEngine;

// This script is attached to collectable items in the game world. It holds a reference to a CollectableItem ScriptableObject
// which contains the details about the item (name, type, value, color, prefab, etc).
//
// The RequireComponent attribute ensures that the GameObject this script is attached to has the specified components.
// We require a Collider and a RigidBody for the collisions/triggers to work properly. We also require a Renderer to apply color.

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class CollectableController : MonoBehaviour
{
    // Reference to a CollectableItem ScriptableObject which contains item details. Notice that this field is private
    // so it won't be accessible from other scripts, but it's serialized so we can assign it in the Unity Inspector.
    // However, I've created a public property below to allow controlled access to it. A csharp property lets us define
    // custom behavior when getting or setting the value. 
    [SerializeField] private CollectableItem _theItemsSO;

    // Public property that wraps the serialized field. This allows other scripts to get or set the CollectableItem
    // while still keeping the field itself private. It also allows us add additional code when we get or set the value.
    public CollectableItem theItemsSO
    {
        // This is a getter. It runs when another script tries to access theItemsSO property. The code in the other script
        // will look something like:
        //    CollectableItem item = collectableController.theItemsSO;
        // As you can see above, it just looks like we are accessing a normal public field, but under the hood, this 
        // getter method below is being called.
        get
        {
            if (_theItemsSO == null)
            {
                Debug.LogError($"CollectableController on {gameObject.name} has a null theItemsSO!");
            }

            return _theItemsSO;
        }

        // This is a setter. It runs when another script tries to assign a value to theItemsSO property. The code in the
        // other script will look something like:
        //    collectableController.theItemsSO = someCollectableItem;
        // Again, it looks like we are just assigning a value to a public field, but under the hood, this setter method 
        // below is being called.
        // All setter methods have an implicit parameter called 'value' which holds the value being assigned.
        set
        {
            _theItemsSO = value;
        }
    }

    // CollectableItem scriptable object's (that's a mouth full) have a color property which I use to distinguish 
    // different items visually. The color will be applied to this object's material. To do that, I cache the Renderer
    // and Material references here.
    private Renderer _renderer;
    private Material _material;

    // Awake is called when the script instance is being loaded. This happens before any Start calls.
    void Awake()
    {                
        // Get the Renderer component attached to this GameObject and store it for later use
        _renderer = GetComponent<Renderer>();

        // Get the Material instance from the Renderer. The first time you access .material, Unity creates an 
        // instance of the material so I want to store it here (otherwise we'd be creating a new instance each time
        // I go use renderer.material).
        _material = _renderer.material;

        ApplyColorFromItem();
    }

    // Apply the color from the assigned CollectableItem to this object's material
    public void ApplyColorFromItem()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            return;

        // In Edit Mode, use sharedMaterial to avoid creating instances that leak
        // In Play Mode, use material instance (already cached in _material)
        Material targetMaterial = Application.isPlaying ? _material : renderer.sharedMaterial;
        
        if (targetMaterial == null)
            return;

        Color color = theItemsSO.color;

        if (targetMaterial.HasProperty("_BaseColor")) // for URP and HDRP shaders
        {
            targetMaterial.SetColor("_BaseColor", color);
        }
        else if (targetMaterial.HasProperty("_Color")) // for standard shader
        {
            targetMaterial.SetColor("_Color", color);
        }
        else
        {
            // Fallback that works for many shaders
            targetMaterial.color = color;
        }
    }

    // Below I am using the #if UNITY_EDITOR directive. Essentilly what this does is tell the compiler to only include the code 
    // within the block *if* the code is being compiled for the Unity Editor. So, when we build the game the code inside this block
    // won't be excluded, which is useful for editor-specific functionality that we don't want in the final build. Alternatively,
    // I could have just left #if UNITY_EDITOR and #endif out, but then the code within the block would be included in the final build.
    // This wouldn't be the end of the world but OnValidate only gets called when we make changes in the editor so it's code that will
    // only run in the editor anyway and never get executed in a build.

    #if UNITY_EDITOR

        // The OnValidate method is called by Unity when:
        //   - you modify Inspector values (in Edit Mode OR Play Mode)
        //   - Scripts are recompiled
        //   - Scene/prefab is loaded
        //   - Object is duplicated or pasted
        //   - You stop Play Mode and values revert
        //   - the script is loaded or a value is changed in the Inspector (in the Editor only).
        // 
        // I'm using here to that when we stop Play Mode the color that was applied from the CollectableItem (in Play mode) remains on the 
        // GameObject. 
        private void OnValidate()
        {            
            if (!Application.isPlaying) // Don't run in play mode so we don't override manual runtime changes
            {
                ApplyColorFromItem();
            }
        }
    #endif
}
