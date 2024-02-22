using RainbowTetris.GameLogic.Blocks;

namespace RainbowTetris.GameLogic;

public class BlockQueue
{
	private readonly Block[] blocks = new Block[]
	{
		new IBlock(),
		new JBlock(),
		new LBlock(),
		new OBlock(),
		new SBlock(),
		new TBlock(),
		new ZBlock()
	};

	public BlockQueue()
	{
		NextBlock = RandomBlock;
	}
	
	private readonly Random random = new Random();
	public Block NextBlock { get; private set; }
	
	private Block RandomBlock => blocks[random.Next(blocks.Length)];

	public Block GetAndUpdate()
	{
		var block = NextBlock;
		do NextBlock = RandomBlock;
		while(block.Id == NextBlock.Id);
		return block;
	}
}