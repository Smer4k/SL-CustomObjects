namespace DONT_TOUCH.Enums
{
    public enum BlockType
    {
        Empty = 0,
        Primitive = 1,
        Light = 2,
        Pickup = 3,
        Workstation = 4,
        Schematic = 5,
        Teleport = 6,
        Locker = 7,
        Text = 8,
        Interactable = 9,
        Waypoint = 10,
        Door = 30, // when merging replace with normal serial number
        Camera = 31,
        ShootingTarget = 32,
        PlayerSpawnPoint = 33,
        Capybara = 34,
        PlayerBlocker = 35,
        CullingParent = 36,
        MirrorPrefab = 37,
        Clutter = 38,
        Trigger = 39,
        AudioPlayer = 40,
        CullingZone = 41,
        Generator = 42,
        CameraTransfer = 43,
    }
}