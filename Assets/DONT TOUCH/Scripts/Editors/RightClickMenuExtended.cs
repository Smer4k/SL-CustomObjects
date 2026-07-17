using DONT_TOUCH.Scripts;
using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEngine;

public class RightClickMenuExtended
{
	[MenuItem("GameObject/🛠️ MER Blocks/Empty (Schematic)", false, -2)]
	private static void CreateEmptyOrSchematic(MenuCommand menuCommand)
	{
		GameObject parent = Selection.activeGameObject;

		if (menuCommand.context as GameObject != null)
			parent = menuCommand.context as GameObject;

		if (parent == null)
		{
			CreateBlock(menuCommand, "Assets/Resources/Blocks/Schematic.prefab");
		}
		else
		{
			CreateBlock(menuCommand, "Assets/Resources/Blocks/Empty.prefab");
		}
	}

	#region Primitives
	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Cube", false, -1)]
	private static void CreateCube(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Cube);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Sphere", false, -1)]
	private static void CreateSphere(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Sphere);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Capsule", false, -1)]
	private static void CreateCapsule(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Capsule);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Cylinder", false, -1)]
	private static void CreateCylinder(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Cylinder);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Plane", false, -1)]
	private static void CreatePlane(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Plane);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Quad", false, -1)]
	private static void CreateQuad(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Quad);

	private static void CreatePrimitive(MenuCommand menuCommand, PrimitiveType primitiveType) => CreateBlock(menuCommand, $"Assets/Resources/Blocks/Primitives/{primitiveType}.prefab");
	#endregion

	#region Lights
	[MenuItem("GameObject/🛠️ MER Blocks/Lights/Directional", false, -1)]
	private static void CreateDirectionalLight(MenuCommand menuCommand) => CreateLight(menuCommand, LightType.Directional);

	[MenuItem("GameObject/🛠️ MER Blocks/Lights/Point", false, -1)]
	private static void CreatePointLight(MenuCommand menuCommand) => CreateLight(menuCommand, LightType.Point);

	[MenuItem("GameObject/🛠️ MER Blocks/Lights/Spot", false, -1)]
	private static void CreateSpotLight(MenuCommand menuCommand) => CreateLight(menuCommand, LightType.Spot);

	[MenuItem("GameObject/🛠️ MER Blocks/Lights/Rectangle", false, -1)]
	private static void CreateRectangleLight(MenuCommand menuCommand) => CreateLight(menuCommand, LightType.Rectangle);

	private static void CreateLight(MenuCommand menuCommand, LightType lightType) => CreateBlock(menuCommand, $"Assets/Resources/Blocks/Lights/{lightType} Light.prefab");
	#endregion

	[MenuItem("GameObject/🛠️ MER Blocks/Pickup", false, -1)]
	private static void CreatePickup(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Pickup.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Workstation", false, -1)]
	private static void CreateWorkstation(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Workstation.prefab");

	// [MenuItem("GameObject/🛠️ MER Blocks/Teleport", false, -1)]
	// private static void CreateTeleport(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Teleporter.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Text", false, -1)]
	private static void CreateText(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Text.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Interactable", false, -1)]
	private static void CreateInteractable(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Interactable.prefab");

	#region Doors
	[MenuItem("GameObject/🛠️ MER Blocks/Doors/Ez", false, -1)]
	private static void CreateEzDoor(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Doors/Ez.prefab");
	[MenuItem("GameObject/🛠️ MER Blocks/Doors/Lcz", false, -1)]
	private static void CreateLczDoor(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Doors/Lcz.prefab");
	[MenuItem("GameObject/🛠️ MER Blocks/Doors/Hcz", false, -1)]
	private static void CreateHczDoor(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Doors/Hcz.prefab");
	[MenuItem("GameObject/🛠️ MER Blocks/Doors/HeavyBulk", false, -1)]
	private static void CreateHeavyBulkDoor(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Doors/HeavyBulk.prefab");
	[MenuItem("GameObject/🛠️ MER Blocks/Doors/Gate", false, -1)]
	private static void CreateGateDoor(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Doors/Gate.prefab");
	#endregion

	#region Lockers

	[MenuItem("GameObject/🛠️ MER Blocks/Lockers/Adrenaline", false, -1)]
	private static void CreateAdrenalineLocker(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Lockers/Adrenaline.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/Lockers/Medkit", false, -1)]
	private static void CreateMedkitLocker(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Lockers/Medkit.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Lockers/Large Gun", false, -1)]
	private static void CreateLargeGunLocker(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Lockers/LargeGun.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Lockers/Experimental Weapon", false, -1)]
	private static void CreateExperimentalWeaponLocker(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Lockers/ExperimentalWeapon.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Lockers/Misc", false, -1)]
	private static void CreateMiscLocker(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Lockers/Misc.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Lockers/Pedestal", false, -1)]
	private static void CreatePedestalLocker(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Lockers/PedestalScp500.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Lockers/Rifle Rack", false, -1)]
	private static void CreateRifleRackLocker(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Lockers/RifleRack.prefab");
	
	#endregion

	#region Shooting Targets

	[MenuItem("GameObject/🛠️ MER Blocks/Shooting Targets/Binary", false, -1)]
	private static void CreateBinaryShootingTarget(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/ShootingTargets/Binary.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Shooting Targets/ClassD", false, -1)]
	private static void CreateClassDShootingTarget(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/ShootingTargets/ClassD.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Shooting Targets/Sport", false, -1)]
	private static void CreateSportShootingTarget(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/ShootingTargets/Sport.prefab");
	
	#endregion

	#region Cameras

	[MenuItem("GameObject/🛠️ MER Blocks/Cameras/Ez", false, -1)]
	private static void CreateEzCamera(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Cameras/Ez.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Cameras/Ez arm", false, -1)]
	private static void CreateEzArmCamera(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Cameras/EzArm.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Cameras/Hcz", false, -1)]
	private static void CreateHczCamera(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Cameras/Hcz.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Cameras/Lcz", false, -1)]
	private static void CreateLczCamera(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Cameras/Lcz.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Cameras/Sz", false, -1)]
	private static void CreateSzCamera(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Cameras/Sz.prefab");

	#endregion
	
	[MenuItem("GameObject/🛠️ MER Blocks/SpawnPoint", false, -1)]
	private static void CreateSpawnPoint(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/SpawnPoint.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Teleporter", false, -1)]
	private static void CreateTeleporter(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Teleporter.prefab");

	
	[MenuItem("GameObject/🛠️ MER Blocks/Capybara", false, -1)]
	private static void CreateCapybara(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Capybara.prefab");

	
	[MenuItem("GameObject/🛠️ MER Blocks/Waypoint", false, -1)]
	private static void CreateWaypoint(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Waypoint.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/PlayerBlocker", false, -1)]
	private static void CreatePlayerBlocker(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/PlayerBlocker.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/CullingParent", false, -1)]
	private static void CreateCullingParent(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/CullingParent.prefab");
	
	#region MirrorPrefabs
	
	[MenuItem("GameObject/🛠️ MER Blocks/MirrorPrefabs/Broken Electrical Box", false, -1)]
	private static void CreateBrokenElectricalBox(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/MirrorPrefabs/BrokenElectricalBox.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/MirrorPrefabs/Simple Boxes", false, -1)]
	private static void CreateSimpleBoxes(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/MirrorPrefabs/SimpleBoxes.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/MirrorPrefabs/Pipes Short", false, -1)]
	private static void CreatePipesShort(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/MirrorPrefabs/PipesShort.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/MirrorPrefabs/Boxes Ladder", false, -1)]
	private static void CreateBoxesLadder(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/MirrorPrefabs/BoxesLadder.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/MirrorPrefabs/Tank-Supported Shelf", false, -1)]
	private static void CreateTankSupportedShelf(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/MirrorPrefabs/TankSupportedShelf.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/MirrorPrefabs/Angled Fences", false, -1)]
	private static void CreateAngledFences(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/MirrorPrefabs/AngledFences.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/MirrorPrefabs/Huge Orange Pipes", false, -1)]
	private static void CreateHugeOrangePipes(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/MirrorPrefabs/HugeOrangePipes.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/MirrorPrefabs/PipesLong", false, -1)]
	private static void CreatePipesLong(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/MirrorPrefabs/PipesLong.prefab");
	
	#endregion
	
	[MenuItem("GameObject/🛠️ MER Blocks/Clutter", false, -1)]
	private static void CreateClutter(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Clutter.prefab");
	
	[MenuItem("GameObject/🛠️ MER Blocks/Trigger", false, -1)]
	private static void CreateTrigger(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Trigger.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/AudioPlayer", false, -1)]
	private static void CreateAudioPlayer(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/AudioPlayer.prefab");

	
	private static void CreateBlock(MenuCommand menuCommand, string prefabPath)
	{
		GameObject instance = SchematicBlock.Create<GameObject>(prefabPath);
		if (instance == null)
			return;

		GameObject parent = Selection.activeGameObject;

		if (menuCommand.context as GameObject != null)
			parent = menuCommand.context as GameObject;

		GameObjectUtility.SetParentAndAlign(instance, parent);

		Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");

		Selection.activeGameObject = instance;
		if (instance.TryGetComponent<SchematicBlock>(out var block))
			block.GenerateId();
	}
}
