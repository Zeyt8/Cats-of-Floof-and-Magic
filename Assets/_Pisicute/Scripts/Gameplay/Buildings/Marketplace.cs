public class Marketplace : Building
{
    public static Pair<int, int>[,] ExchangeRate =
    {
        //F                          W                          Sn                         Sl                         Sf                         G
        { null                     , new Pair<int, int>(5, 1) , new Pair<int, int>(5, 1) , new Pair<int, int>(10, 1), new Pair<int, int>(15, 1), new Pair<int, int>(15, 1) }, //F
        { new Pair<int, int>(1, 5) , null                     , new Pair<int, int>(2, 1) , new Pair<int, int>(5, 1) , new Pair<int, int>(10, 1), new Pair<int, int>(10, 1) }, //W
        { new Pair<int, int>(1, 5) , new Pair<int, int>(2, 1) , null                     , new Pair<int, int>(5, 1) , new Pair<int, int>(10, 1), new Pair<int, int>(10, 1) }, //Sn
        { new Pair<int, int>(1, 10), new Pair<int, int>(1, 5) , new Pair<int, int>(1, 5) , null                     , new Pair<int, int>(2, 1) , new Pair<int, int>(2, 1)  }, //Sl
        { new Pair<int, int>(1, 15), new Pair<int, int>(1, 10), new Pair<int, int>(1, 10), new Pair<int, int>(2, 1) , null                     , new Pair<int, int>(2, 1)  }, //Sf
        { new Pair<int, int>(1, 15), new Pair<int, int>(1, 10), new Pair<int, int>(1, 10), new Pair<int, int>(2, 1) , new Pair<int, int>(2, 1) , null                      }  //G
    };
}
