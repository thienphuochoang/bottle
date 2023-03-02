using UnityEngine;
namespace Bottle.ProceduralMeshes.Tree
{
    struct TrunkData
    {
        public Mesh trunkMesh;
        public TrunkRingData[] nextTrunkRingDatas;
        public TrunkData(Mesh trunkMesh, 
            TrunkRingData firstFollowingTrunkRingData, TrunkRingData secondFollowingTrunkRingData)
        {
            this.trunkMesh = trunkMesh;
            this.nextTrunkRingDatas = new TrunkRingData[2];
            this.nextTrunkRingDatas[0] = firstFollowingTrunkRingData;
            this.nextTrunkRingDatas[1] = secondFollowingTrunkRingData;
        }
    }
}
