using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Gamekit3D.WorldBuilding
{
    [SelectionBase]
    [CustomEditor(typeof(MegaSmartObject))]
    public partial class MegaSmartObjectEditor : Editor
    {
        BoxBoundsHandle bbh;
        void OnEnable()
        {
            bbh = new BoxBoundsHandle();
        }

        void OnSceneGUI()
        {
            var mso = target as MegaSmartObject;
            using (new Handles.DrawingScope(Color.yellow, mso.transform.localToWorldMatrix))
            {
                bbh.center = mso.bounds.center;
                bbh.size = mso.bounds.size;
                using (var cc = new EditorGUI.ChangeCheckScope())
                {
                    bbh.DrawHandle();
                    if (cc.changed)
                    {
                        var rotation = mso.transform.rotation;
                        mso.transform.rotation = Quaternion.identity;
                        mso.parts.SetActive(true);
                        while (mso.generated.childCount > 0)
                            DestroyImmediate(mso.generated.GetChild(0).gameObject);
                        mso.bounds = new Bounds(bbh.center, bbh.size);
                        var min = mso.bounds.min;
                        var max = mso.bounds.max;
                        var size = mso.originBounds.size;
                        var volume = (max - min);
                        var steps = new Vector3Int(Mathf.FloorToInt(volume.x / size.x), Mathf.FloorToInt(volume.y / size.y), Mathf.FloorToInt(volume.z / size.z));
                        var cursor = Vector3Int.zero;
                        var terminator = 9999;
                        for (var y = min.y; y < max.y; y += size.y, cursor.y++)
                        {
                            cursor.x = 0;
                            for (var x = min.x; x < max.x; x += size.x, cursor.x++)
                            {
                                cursor.z = 0;
                                for (var z = min.z; z < max.z; z += size.z, cursor.z++)
                                {
                                    terminator--;
                                    if (terminator <= 0) return;
                                    var b = new Bounds();
                                    var bl = new Vector3(x, y, z);
                                    var tr = bl + size;
                                    b.center = Vector3.Lerp(bl, tr, 0.5f);
                                    b.size = size;
                                    // var centerPrefab = mso.parts.center;
                                    // if (mso.parts.alternateCenters.Length > 0)
                                    // {
                                    //     centerPrefab = mso.parts.alternateCenters[Random.Range(0, mso.parts.alternateCenters.Length)];
                                    // }
                                    var part = Instantiate(mso.parts.center);
                                    part.rotation = Quaternion.identity;
                                    part.parent = mso.generated;
                                    var offset = (mso.parts.center.localPosition - mso.originBounds.center);
                                    part.localPosition = b.center + offset;
                                    if (cursor.z == 0)
                                    {
                                        var bottom = Instantiate(mso.parts.bottom);
                                        bottom.parent = mso.generated;
                                        bottom.localRotation = mso.parts.bottom.localRotation;
                                        offset = (mso.parts.bottom.localPosition - mso.originBounds.center);
                                        bottom.localPosition = b.center + offset;
                                        if (cursor.x == 0)
                                        {
                                            var bottomLeft = Instantiate(mso.parts.bottomLeft);
                                            bottomLeft.parent = mso.generated;
                                            bottomLeft.localRotation = mso.parts.bottomLeft.localRotation;
                                            offset = (mso.parts.bottomLeft.localPosition - mso.originBounds.center);
                                            bottomLeft.localPosition = b.center + offset;
                                        }
                                        if (cursor.x == steps.x)
                                        {
                                            var bottomRight = Instantiate(mso.parts.bottomRight);
                                            bottomRight.parent = mso.generated;
                                            bottomRight.localRotation = mso.parts.bottomRight.localRotation;
                                            offset = (mso.parts.bottomRight.localPosition - mso.originBounds.center);
                                            bottomRight.localPosition = b.center + offset;
                                        }
                                    }
                                    if (cursor.z == steps.z)
                                    {
                                        var top = Instantiate(mso.parts.top);
                                        top.parent = mso.generated;
                                        top.localRotation = mso.parts.top.localRotation;
                                        offset = (mso.parts.top.localPosition - mso.originBounds.center);
                                        top.localPosition = b.center + offset;
                                        if (cursor.x == 0)
                                        {
                                            var topLeft = Instantiate(mso.parts.topLeft);
                                            topLeft.parent = mso.generated;
                                            topLeft.localRotation = mso.parts.bottomLeft.localRotation;
                                            offset = (mso.parts.topLeft.localPosition - mso.originBounds.center);
                                            topLeft.localPosition = b.center + offset;
                                        }
                                        if (cursor.x == steps.x)
                                        {
                                            var topRight = Instantiate(mso.parts.topRight);
                                            topRight.parent = mso.generated;
                                            topRight.localRotation = mso.parts.topRight.localRotation;
                                            offset = (mso.parts.topRight.localPosition - mso.originBounds.center);
                                            topRight.localPosition = b.center + offset;
                                        }
                                    }
                                    if (cursor.x == 0)
                                    {
                                        var left = Instantiate(mso.parts.left);
                                        left.parent = mso.generated;
                                        left.localRotation = mso.parts.left.localRotation;
                                        offset = (mso.parts.left.localPosition - mso.originBounds.center);
                                        left.localPosition = b.center + offset;
                                    }
                                    if (cursor.x == steps.x)
                                    {
                                        var right = Instantiate(mso.parts.right);
                                        right.parent = mso.generated;
                                        right.localRotation = mso.parts.right.localRotation;
                                        offset = (mso.parts.right.localPosition - mso.originBounds.center);
                                        right.localPosition = b.center + offset;
                                    }
                                }
                            }
                        }
                        mso.transform.rotation = rotation;
                        var bc = mso.gameObject.GetComponent<BoxCollider>();
                        if (bc == null)
                            bc = mso.gameObject.AddComponent<BoxCollider>();
                        var bcb = mso.generated.gameObject.GetLocalBounds();
                        bc.center = bcb.center;
                        bc.size = bcb.size;
                        mso.parts.SetActive(false);
                    }
                }
            }
        }
    }
}