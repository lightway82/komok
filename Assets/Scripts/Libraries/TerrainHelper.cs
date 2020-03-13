using UnityEngine;

public class TerrainHelper 
{
 /// <summary>
 /// Получает массив значений весов смешивания текстурных слоев на терайне в точке соответствующей позиции worldPos( эта позиция проецируется на терайн вниз, Y Не учитывается)
 /// Индексы массива соответствуют индексам текстуры
 /// </summary>
 /// <param name="worldPos">Координаты  точки в мировом пространстве из которой определяем текстуры</param>
 /// <param name="terrain">Ссылка на проверяемый терайн</param>
 /// <returns></returns>
   public static float[] GetTexturesMix(Vector3 worldPos,  Terrain terrain) {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        // расчитывается координаты ячейки на терайне в которую попадает позиция, y не учитывается
        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
        // получается splat data для этой cell как  1x1xN 3d array (где N = количество текстур)
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX,mapZ,1,1);
        // извлечение значений смешивания в одномерный массив:
        float[] cellMix = new float[splatmapData.GetUpperBound(2)+1];
        for (int n=0; n < cellMix.Length; ++n)
        {
            cellMix[n] = splatmapData[0,0,n];    
        }
        return cellMix;        
    }


 /// <summary>
 /// Получает преобладающую текстуру на терайне(имеет наибольший вес при смешивании). Возвращает ее индекс
 /// </summary>
 /// <param name="worldPos"></param>
 /// <param name="terrain"></param>
 /// <returns></returns>
 public static int GetMainTexture(Vector3 worldPos,  Terrain terrain) {
        // returns the zero-based index of the most dominant texture
        // on the main terrain at this world position.
        float[] mix = GetTexturesMix(worldPos, terrain);
        float maxMix = 0;
        int maxIndex = 0;
        // loop through each mix value and find the maximum
        for (int n=0; n < mix.Length; ++n)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }
    
 /* 
public Texture2D getTerrainTextureAt( Vector3 position )
{
  // Set up:
  Texture2D retval=null;
  Vector3 TS; // terrain size
  Vector2 AS; // control texture size

  TS = Terrain.activeTerrain.terrainData.size;
  AS.x = Terrain.activeTerrain.terrainData.alphamapWidth;
  AS.y = Terrain.activeTerrain.terrainData.alphamapHeight;


  // Lookup texture we are standing on:
  int AX = (int)( ( position.x/TS.x )*AS.x + 0.5f );
  int AY = (int)( ( position.z/TS.z )*AS.y + 0.5f );
  float[,,] TerrCntrl = Terrain.activeTerrain.terrainData.GetAlphamaps(AX, AY,1 ,1);
   
  TerrainData TD = Terrain.activeTerrain.terrainData;
   
  for( int i = 0; i < TD.splatPrototypes.Length; i++ )
  {
      if( TerrCntrl[0,0,i] > .5f )
      {
          retval    =    TD.splatPrototypes[i].texture;
      }
       
  }
   
   
  return retval;
}



void Start()
{
mTerrainData = Terrain.activeTerrain.terrainData;
alphamapWidth = mTerrainData.alphamapWidth;
alphamapHeight = mTerrainData.alphamapHeight;

mSplatmapData = mTerrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
mNumTextures = mSplatmapData.Length / (alphamapWidth * alphamapHeight);
}

private Vector3 ConvertToSplatMapCoordinate(Vector3 playerPos)
{
Vector3 vecRet = new Vector3();
Terrain ter = Terrain.activeTerrain;
Vector3 terPosition = ter.transform.position;
vecRet.x = ((playerPos.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
vecRet.z = ((playerPos.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
return vecRet;
}

void Update()
{
int terrainIdx = GetActiveTerrainTextureIdx();
PlayFootStepSound(terrainIdx);
}

int GetActiveTerrainTextureIdx()
{
Vector3 playerPos = PlayerController.Instance.position;
Vector3 TerrainCord = ConvertToSplatMapCoordinate(playerPos);
int ret = 0;
float comp = 0f;
for (int i = 0; i < mNumTextures; i++)
{
  if (comp < mSplatmapData[(int)TerrainCord.z, (int)TerrainCord.x, i])
      ret = i;
}
return ret;
}


*/
}
