public class ResetCommand : Command {

	public override void Execute ()
	{
		GameController.instance.ResetCells();
	}
}
