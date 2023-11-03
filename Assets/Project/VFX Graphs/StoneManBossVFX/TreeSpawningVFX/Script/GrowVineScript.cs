using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowVineScript : MonoBehaviour
{
	public List<MeshRenderer> growVinesMeshes;
	[SerializeField] private float timeToGrow = 5;
	[SerializeField] private float refershRate = 0.05f;
	[Range(0f, 1f)] public float minGrow = 0.5f;
	[Range(0f, 1f)] public float maxGrow = 0.97f;
	[SerializeField] private List<Material> growVineMaterials = new List<Material>();

	[SerializeField] private bool fullyGrown;

	// Start is called before the first frame update
	void Awake()
	{
		for (int i = 0; i < growVinesMeshes.Count; i++)
		{
			for (int j = 0; j < growVinesMeshes[i].materials.Length; j++)
			{
				if (growVinesMeshes[i].materials[j].HasProperty("_Grow"))
				{
					growVinesMeshes[i].materials[j].SetFloat("_Grow", minGrow);
					growVineMaterials.Add(growVinesMeshes[i].materials[j]);
				}
			}
		}
	}

	[ContextMenu("GrowVines")]
	public void GrowVines()
	{
		for (int i = 0; i < growVineMaterials.Count; i++)
		{
			StartCoroutine(GrowVinesRoutine(growVineMaterials[i]));
		}
	}

	private IEnumerator GrowVinesRoutine(Material mat)
	{
		float growValue = mat.GetFloat("_Grow");
		while (growValue < maxGrow)
		{
			growValue += 1 / (timeToGrow / refershRate);
			mat.SetFloat("_Grow", growValue);

			yield return new WaitForSeconds(refershRate);
		}
	}

	[ContextMenu("ReverseGrowVines")]
	public void ReverseGrowVines()
	{
		for (int i = 0; i < growVineMaterials.Count; i++)
		{
			StartCoroutine(ReverseGrowVinesRoutine(growVineMaterials[i]));
		}
	}

	private IEnumerator ReverseGrowVinesRoutine(Material mat)
	{
		float growValue = mat.GetFloat("_Grow");
		while (growValue > minGrow)
		{
			growValue -= 1 / (timeToGrow / refershRate);
			mat.SetFloat("_Grow", growValue);

			yield return new WaitForSeconds(refershRate);
		}
	}
}
