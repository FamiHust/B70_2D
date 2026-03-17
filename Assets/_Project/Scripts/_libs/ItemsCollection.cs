using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsCollection : ScriptableObject
{

    [System.Serializable]
    public class Configuration
    {
        public float buildTime = 0;
        public bool isCharacter;
        public float speed;
        public float attackRange = 0;
        public float defenceRange = 0;
        public float healthPoints;
        public float hitPoints;
		public float productionRate;
		public string product;
        public int productPrice = 100;          // GIÁ BUILD
        public int price = 100;          // GIÁ BUILD
        public string resourceType = "gold";
    }

    [System.Serializable]
    public class ItemData
    {
        public int id;
        public string name;
		public Texture2D thumb;

        public int gridSize = 4;
        public Configuration configuration = new Configuration();

        // store textures directly for each state
        public List<Texture2D> idleSpriteTextures;
        public List<Texture2D> walkSpriteTextures;
        public List<Texture2D> attackSpriteTextures;
        public List<Texture2D> destroyedSpriteTextures;



        public ItemData()
        {
            this.idleSpriteTextures = new List<Texture2D>();
            this.walkSpriteTextures = new List<Texture2D>();
            this.attackSpriteTextures = new List<Texture2D>();
            this.destroyedSpriteTextures = new List<Texture2D>();
        }

        public void AddSprite(SpriteCollection.SpriteData sprite, Common.State state)
        {
            if (sprite == null) return;
            Texture2D tex = sprite.bottomTexture != null ? sprite.bottomTexture.texture : null;
            if (tex == null)
            {
                if (sprite.bottomRightTexture != null) tex = sprite.bottomRightTexture.texture;
                if (tex == null && sprite.rightTexture != null) tex = sprite.rightTexture.texture;
                if (tex == null && sprite.topRightTexture != null) tex = sprite.topRightTexture.texture;
                if (tex == null && sprite.topTexture != null) tex = sprite.topTexture.texture;
            }
            if (tex == null) return;

            List<Texture2D> sprites = idleSpriteTextures;
            switch (state)
            {
                case Common.State.IDLE:
                    sprites = idleSpriteTextures;
                    break;
                case Common.State.WALK:
                    sprites = walkSpriteTextures;
                    break;
                case Common.State.ATTACK:
                    sprites = attackSpriteTextures;
                    break;
                case Common.State.DESTROYED:
                    sprites = destroyedSpriteTextures;
                    break;
            }

            if (!sprites.Contains(tex)) sprites.Add(tex);
        }

        public void RemoveSprite(SpriteCollection.SpriteData sprite, Common.State state)
        {
            if (sprite == null) return;
            Texture2D tex = sprite.bottomTexture != null ? sprite.bottomTexture.texture : null;
            if (tex == null)
            {
                if (sprite.bottomRightTexture != null) tex = sprite.bottomRightTexture.texture;
                if (tex == null && sprite.rightTexture != null) tex = sprite.rightTexture.texture;
                if (tex == null && sprite.topRightTexture != null) tex = sprite.topRightTexture.texture;
                if (tex == null && sprite.topTexture != null) tex = sprite.topTexture.texture;
            }
            if (tex == null) return;

            List<Texture2D> sprites = idleSpriteTextures;
            switch (state)
            {
                case Common.State.IDLE:
                    sprites = idleSpriteTextures;
                    break;
                case Common.State.WALK:
                    sprites = walkSpriteTextures;
                    break;
                case Common.State.ATTACK:
                    sprites = attackSpriteTextures;
                    break;
                case Common.State.DESTROYED:
                    sprites = destroyedSpriteTextures;
                    break;
            }

            if (sprites.Contains(tex)) sprites.Remove(tex);
        }

        // Return sprite ids by mapping stored textures to sprite entries in SpriteCollection
        public List<int> GetSprites(Common.State state)
        {
            List<Texture2D> textures = GetSpriteTextures(state);
            List<int> ids = new List<int>();
            if (textures == null) return ids;
            foreach (Texture2D tex in textures)
            {
                SpriteCollection.SpriteData sprite = Sprites.GetSpriteByTexture(tex);
                if (sprite != null && !ids.Contains(sprite.id))
                {
                    ids.Add(sprite.id);
                }
            }
            return ids;
        }

        // Return stored Texture2D list for a given state
        public List<Texture2D> GetSpriteTextures(Common.State state)
        {
            switch (state)
            {
                case Common.State.IDLE:
                    return idleSpriteTextures;
                case Common.State.WALK:
                    return walkSpriteTextures;
                case Common.State.ATTACK:
                    return attackSpriteTextures;
                case Common.State.DESTROYED:
                    return destroyedSpriteTextures;
            }

            return idleSpriteTextures;
        }


    }

    public List<ItemData> list = new List<ItemData>();

    public void AddNewItem()
    {
        ItemData newItemData = new ItemData();
        newItemData.id = this._GetUnusedId();
        newItemData.name = "New Item";

        this.list.Add(newItemData);
    }

    public void RemoveItem(int index)
    {
        this.list.RemoveAt(index - 1);
    }

    private int _GetUnusedId()
    {
        int id = Random.Range(1000, 9999);
        for (int index = 0; index < this.list.Count; index++)
        {
            if (id == list[index].id)
            {
                return _GetUnusedId();
            }
        }
        return id;
    }
}