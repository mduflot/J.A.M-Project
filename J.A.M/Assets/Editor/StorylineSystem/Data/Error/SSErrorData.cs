using UnityEngine;

namespace SS.Data.Error
{
    public class SSErrorData
    {
        public Color Color { get; set; }

        public SSErrorData()
        {
            GenerateRandomColor();
        }
        
        private void GenerateRandomColor()
        {
            Color = new Color32((byte) Random.Range(65, 256), (byte) Random.Range(50, 176), (byte) Random.Range(50, 176), 255);
        }
    }
}