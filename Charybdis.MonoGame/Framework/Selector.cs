using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.MonoGame.Framework
{
    public static class Selector
    {
        public static List<GameObject> Selection { get; private set; }

        static Selector()
        {
            Selection = new List<GameObject>();
        }

        public static void Select(GameObject go)
        {
            if (go.SelectionEnabled && !go.Selected)
            {
                //if (!Selection.Contains(s)) //Can skip this due to ISelectable.Selected, as long as we can trust that value.
                go.Selected = true;
                Selection.Add(go);
            }
        }

        public static void Deselect(GameObject go)
        {
            if (go.SelectionEnabled && go.Selected)
            {
                //if (Selection.Contains(s)) //Can skip this due to ISelectable.Selected, as long as we can trust that value.
                go.Selected = false;
                Selection.Remove(go);
            }
        }

        public static void DeselectAll()
        {
            foreach (var go in Selection)
                Deselect(go);
        }

        public static void SelectAll()
        {
            foreach (var go in Selection)
                Select(go);
        }

        public static void SelectMany(Func<GameObject, bool> predicate)
        {
            foreach (var go in Selection)
                if (predicate(go))
                    Select(go);
        }

        public static void SelectMany(IEnumerable<GameObject> gameObjects)
        {
            foreach (var go in gameObjects)
                Select(go);
        }

        public static int Count
        {
            get
            {
                return Selection.Count;
            }
        }
    }
}
