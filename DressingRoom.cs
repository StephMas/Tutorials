using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressingRoom : MonoBehaviour
{
    //The armature of the character you're adding parts to    
    public Transform newArmature;
    //The name of the root bone for that character's armature.
    public string rootBoneName = "Hips";

    //The part you want to add to the character
    public GameObject partToAdd = null;
    public GameObject partAdded = null;
    public Material materialForNewPart = null;

    public List<GameObject> partsAdded = new List<GameObject>();

    public void OverwritePartChoice(GameObject _partToAdd)
    {
        partToAdd = _partToAdd;
        Reskin();
    }

    public GameObject ChoosePart(GameObject _partToAdd)
    {

        if (partToAdd != null)
        {
            partAdded = partToAdd;
            return partToAdd;
            
        }
        else
        {
            Debug.Log("No new clothing has been assigned");
            partToAdd = null;
            partAdded = null;
          
            return null;
        }
    }

    public void Reskin()
    {
        //First check to see if your character is already wearing the item
        bool alreadyWearing = CheckIfWearing(partToAdd);

        if (alreadyWearing)
        {
            Debug.Log("The character is already wearing that item.");
            return;
        }
        else
        {
            GameObject clothing = ChoosePart(partToAdd);

            if (newArmature == null)
            {
                Debug.Log("No new armature assigned");
                return;
            }

            if (newArmature.Find(rootBoneName) == null)
            {
                Debug.Log("Root bone not found");
                return;
            }

            //Debug.Log("Reassingning bones");

            SkinnedMeshRenderer rend = clothing.GetComponent<SkinnedMeshRenderer>();

            Transform[] bones = rend.bones;

            rend.rootBone = newArmature.Find(rootBoneName);

            Transform[] children = newArmature.GetComponentsInChildren<Transform>();

            for (int i = 0; i < bones.Length; i++)
                for (int a = 0; a < children.Length; a++)
                    if (bones[i].name == children[a].name)
                    {
                        bones[i] = children[a];
                        break;
                    }

            rend.bones = bones;
            DuplicateReassignedMesh(clothing);
        }


    }

    public void DuplicateReassignedMesh(GameObject duplicatedObject)
    {
        GameObject duplicate = Instantiate(duplicatedObject);
        MoveMeshToNewCharacter(duplicate);

        //Set up the new material
        if (materialForNewPart)
        {
            SkinnedMeshRenderer skinMeshRend = duplicate.GetComponent<SkinnedMeshRenderer>();
            Material oldMat = skinMeshRend.material;

            skinMeshRend.material = materialForNewPart;
        }
        else
        {
            Debug.Log("No material has been defined.");
            //do nothing
        }
    }

    public void MoveMeshToNewCharacter(GameObject _duplicatedObject)
    {
        //Find the armature parent
        Transform armatureParent = newArmature.parent;
        _duplicatedObject.transform.SetParent(armatureParent);

        NewClothingReference(_duplicatedObject);
    }


    public void NewClothingReference(GameObject _duplicatedObject)
    {
        if (partsAdded.Contains(_duplicatedObject))
        {
            Debug.Log("The part is already in the list.");

        }
        else
        {
            partsAdded.Add(_duplicatedObject);
          }
    }

    public bool CheckIfWearing(GameObject _partAdded)
    {
        string nameOfPart = _partAdded.name;

        foreach (GameObject _part in partsAdded)
        {
            if (_part.name == _partAdded.name + "(Clone)")
            {
                Debug.Log("The character is already wearing that item.");
                return true;
            }
            else
            {
                Debug.Log("The character is not wearing that item.");
                return false;
            }
        }

        return false;
    }

    public GameObject GetWornItemFromOriginalNameReference(GameObject _originalPart)
    {
        string nameOfPart = _originalPart.name;

        foreach (GameObject _part in partsAdded)
        {
            if (_part.name == _originalPart.name + "(Clone)")
            {
                Debug.Log("The item is " + _part.name);
                return _part;
            }
            else
            {
                Debug.Log("The character is not wearing that item.");
                return null;
            }
        }

        return null;
    }


    public void DeleteClothing(GameObject _partToRemove)
    {
        //First check to see if you're already wearing the item
        bool alreadyWearing = CheckIfWearing(_partToRemove);
        GameObject cloneOfPart = GetWornItemFromOriginalNameReference(_partToRemove);

        if (alreadyWearing)
        {                        
            DestroyImmediate(cloneOfPart);
            partsAdded.Remove(cloneOfPart);
        }        
        else
        {
            Debug.Log("The character is not wearing that item.");
            return;
        }
    }
}


