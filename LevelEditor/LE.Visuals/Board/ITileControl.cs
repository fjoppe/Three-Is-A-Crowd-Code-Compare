
namespace TIAC.Visuals.Board
{
    public interface ITileControl
    {
        HexagonTile CurrentTile { get; set; }

        double X { get; }
        double Y { get; }

        void SetLinkCount();

        TileType TileType { get; }
    }
}
