namespace Woods {
    public class Block {
        public int id;
        public string name;
        public float opacity;
        public float lightIntensity;
        public bool solid;
        public int maxHealth;
        public int produceOnBreak;
        public int giveOnBreak;
        
    }

    public enum BlockIDs {
        Air,
        Tree,
        Rock,
        Planks,
        Campfire,
        Torch,
        Glass,
        DoorFull,
        DoorHalf,
        Stump,
        Log,
        Sand,
        Bench
    }
}