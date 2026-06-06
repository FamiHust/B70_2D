using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesScript : MonoBehaviour {

	/* prefabs */
	public GameObject GoldCollectionParticle;
	public GameObject ElixirCollectionParticle;
	public GameObject HappyCollectionParticle;
	public GameObject EducationCollectionParticle;
	public GameObject DestructionParticle;

	private BaseItemScript _baseItem;

    public void SetData(BaseItemScript baseItem)
    {
        this._baseItem = baseItem;
    }
    

	public void ShowCollectionParticle(string type)
    {
		Vector3 position = this._baseItem.GetPosition() + this._baseItem.GetSize() / 2;
		if (type == "gold")
			SceneManager.instance.ShowParticle(this.GoldCollectionParticle, position);
		else if (type == "elixir")
			SceneManager.instance.ShowParticle(this.ElixirCollectionParticle, position);
		else if (type == "happy")
			SceneManager.instance.ShowParticle(this.HappyCollectionParticle, position);
		else if (type == "education" || type == "edu" || type == "academic")
			SceneManager.instance.ShowParticle(this.EducationCollectionParticle, position);
    }

	public void ShowDestructionParticle()
	{
		Vector3 position = this._baseItem.GetPosition() + this._baseItem.GetSize() / 2;
		SceneManager.instance.ShowParticle(this.DestructionParticle, position);
	}
}
