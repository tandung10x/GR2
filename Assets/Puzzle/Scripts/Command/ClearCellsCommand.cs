public class ClearCellsCommand : Command {

	public override void Execute ()
	{
		GameController.instance.ClearCell (GameManager.instance.countToClear);
	}
}
