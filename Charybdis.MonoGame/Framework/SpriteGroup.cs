using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.MonoGame
{
    public class SpriteGroup : Drawable2, IList<Sprite>
    {
        List<Sprite> Sprites = new();

        public override void Draw(SpriteBatch spriteBatch, Vec2 offset)
        {
            for (int i = 0; i < Sprites.Count; i++)
                Sprites[i].Draw(spriteBatch, offset);
        }

        public Sprite this[int index] { get => ((IList<Sprite>)Sprites)[index]; set => ((IList<Sprite>)Sprites)[index] = value; }

        public int Count => ((ICollection<Sprite>)Sprites).Count;

        public bool IsReadOnly => ((ICollection<Sprite>)Sprites).IsReadOnly;

        public void Add(Sprite item)
        {
            ((ICollection<Sprite>)Sprites).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Sprite>)Sprites).Clear();
        }

        public bool Contains(Sprite item)
        {
            return ((ICollection<Sprite>)Sprites).Contains(item);
        }

        public void CopyTo(Sprite[] array, int arrayIndex)
        {
            ((ICollection<Sprite>)Sprites).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Sprite> GetEnumerator()
        {
            return ((IEnumerable<Sprite>)Sprites).GetEnumerator();
        }

        public int IndexOf(Sprite item)
        {
            return ((IList<Sprite>)Sprites).IndexOf(item);
        }

        public void Insert(int index, Sprite item)
        {
            ((IList<Sprite>)Sprites).Insert(index, item);
        }

        public bool Remove(Sprite item)
        {
            return ((ICollection<Sprite>)Sprites).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Sprite>)Sprites).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Sprites).GetEnumerator();
        }
    }
}
