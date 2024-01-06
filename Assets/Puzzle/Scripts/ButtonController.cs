using UnityEngine;

public class ButtonController : MonoBehaviour {

	public int i;
	public int j;

 	public void MovingCell () {
		if (HexagonController.currentI != -1 && HexagonController.currentJ != -1) {
			GameController.instance.StartCoroutine(GameController.ChangePos (HexagonController.currentI, HexagonController.currentJ, i, j));
			HexagonController.currentI = -1;
			HexagonController.currentJ = -1;
		}
	}
}
