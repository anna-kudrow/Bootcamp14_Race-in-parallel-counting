

const int THREADS_NUMBER = 4; // число потоков
const int N = 100000;         // размер массива 
object locker = new object();


// int[] array = {0,20,3,2,1,5,29,10,1};

// int []sortedArray = CountingSortExtended(array);

Random rand = new Random();
int [] resSerial = new int [N].Select(r => rand.Next(0,5)).ToArray();
int [] resParallel = new int [N]; 

Array.Copy(resSerial, resParallel, N);

CountingSortExtended(resSerial);
PrepareParallelCountingSort(resParallel);
Console.WriteLine(EqualityMatrix(resSerial,resParallel));



void PrepareParallelCountingSort(int[] inputArray)
{
    int max = inputArray.Max();
    int min = inputArray.Min();

    int offset = -min;
    int [] counters = new int[max + offset + 1];
    
    int eachThreadCalc = N/THREADS_NUMBER; // количество вычислений для каждого потока
    var threadsParall = new List<Thread>();

    for (int i = 0; i < THREADS_NUMBER; i++)
    {
        int startPos = i * eachThreadCalc;
        int endPos = (i + 1) * eachThreadCalc;
        if (i == THREADS_NUMBER -1) endPos = N;
        threadsParall.Add(new Thread(() => CountingSortParallel(inputArray, counters, offset, startPos, endPos)));
        threadsParall[i].Start();
    }

    foreach(var thread in threadsParall)
    {
        thread.Join();
    }

    int index = 0;
    for (int i = 0; i < counters.Length; i++)
    {
        for (int j = 0; j < counters[i]; j++)
        {
            inputArray[index] = i - offset;
            index++;
        }
    }


}
 void CountingSortParallel(int [] inputArray, int [] counters, int offset, int startPos, int endPos)
 {
    for (int i = startPos; i < endPos; i++)
    {
        lock (locker)
        {
            counters[inputArray[i] + offset] ++;
        } 
    }
 }



int [] CountingSortExtended (int[] inputArray)
{
    int max = inputArray.Max();
    int min = inputArray.Min();

    int offset = -min;
    int[] counters = new int [max + offset + 1];

    for (int i = 0; i < inputArray.Length; i++)
    {
        counters[inputArray[i] + offset]++;
    }

    int index = 0;

    for (int i = 0; i < counters.Length; i++)
    {
        for (int j = 0; j < counters[i]; j++)
        {
            inputArray[index] = i - offset;
            index++;
        }
    }

    return inputArray;
}

bool EqualityMatrix(int[] fmatrix, int[] smatrix)
{
    bool res = true;
    for (int i = 0; i < N; i++)
    {
        res = res && (fmatrix[i] == smatrix[i]);
    }
    return res;
}
