﻿using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SanyoniLib.UnityEngineExtensions
{
    public static class GameObjectExtensions
    {
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            // Non recursive, non allocating traversal
            Transform goTransform = gameObject.transform;
            if (goTransform.childCount > 0)
            {
                WalkHeirarchyAndDo(goTransform, (go) => { go.layer = layer; });
            }
        }

        public static void DoRecursively(this GameObject gameObject, Action<GameObject> action)
        {
            if (gameObject.transform.childCount > 0)
            {
                WalkHeirarchyAndDo(gameObject.transform, action);
            }

        }

        /// <summary>
        /// Walks the heirarchy from <paramref name="root"/>, look at all its children and change the layer.
        /// Non-allocating and non-recursive  
        /// </summary>
        /// <param name="root">The root object to start our search from</param>
        /// <param name="layer">The layer to set the game object too</param>
        private static void WalkHeirarchyAndDo([NotNull] Transform root, Action<GameObject> action)
        {
            if (root.childCount == 0)
            {
                throw new InvalidOperationException("Root transform has no children");
            }

            Transform workingTransform = root.GetChild(0);

            // Work until we get back to the root
            while (workingTransform != root)
            {
                action(workingTransform.gameObject);

                // Get children if we have
                if (workingTransform.childCount > 0)
                {
                    workingTransform = workingTransform.GetChild(0);
                }
                // No children, look for siblings
                else
                {
                    // Set to our sibling
                    if (!TryGetNextSibling(ref workingTransform))
                    {
                        // Otherwise walk up parents and find THEIR next sibling
                        workingTransform = workingTransform.parent;

                        while (workingTransform != root &&
                               !TryGetNextSibling(ref workingTransform))
                        {
                            workingTransform = workingTransform.parent;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Tries to advance to a sibling of <paramref name="transform"/>
        /// </summary>
        /// <param name="transform">The transform whose siblings we're looking for</param>
        /// <returns>True if we had a sibling. <paramref name="transform"/> will now refer to it.</returns>
        private static bool TryGetNextSibling([NotNull] ref Transform transform)
        {
            Transform parent = transform.parent;
            int siblingIndex = transform.GetSiblingIndex();

            // Get siblings if we don't have children
            if (parent.childCount > siblingIndex + 1)
            {
                transform = parent.GetChild(siblingIndex + 1);
                return true;
            }

            return false;
        }

    }

}
