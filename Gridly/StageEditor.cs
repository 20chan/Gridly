using Microsoft.Xna.Framework;

namespace Gridly
{
    public class StageEditor : DefaultPartEditor
    {
        Stage stage;
        public StageEditor(string path)
        {
            stage = new Stage();
            stage.Deserialize(this, SerializeHelper.LoadFromFile(path));
        }
    }
}
