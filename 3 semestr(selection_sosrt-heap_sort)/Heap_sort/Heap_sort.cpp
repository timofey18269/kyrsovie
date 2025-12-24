#include <iostream>
#include <fstream>
#include <string>
#include <vector> 
#include <map> 
#include <array> 
#include <chrono>
#include <iomanip>
#include <sstream>

using namespace std;

int priveous_largest, largest, l, r;

void heapify(int arr[], int k, int i)
{
    priveous_largest = -1, largest = i, l, r;

    while (priveous_largest != largest) {
        priveous_largest = largest;
        l = 2 * largest + 1;
        r = 2 * largest + 2;

        if (l < k && arr[l] > arr[largest])
            largest = l;

        if (r < k && arr[r] > arr[largest])
            largest = r;

        if (priveous_largest != largest)
        {
            swap(arr[priveous_largest], arr[largest]);
        }
    }
}

void heapSort(int arr[], int n)
{
    for (int i = n / 2 - 1; i >= 0; i--)
        heapify(arr, n, i);

    for (int i = n - 1; i >= 0; i--)
    {
        swap(arr[0], arr[i]);
        heapify(arr, i, 0);
    }
}
int main()
{
    const vector<int> test_array_sizes = { 10,20,50,100,200,500,1000,2000,5000,10000 };
    int data_set_count;
    string line;
    int n_start, el_number;
    auto start = chrono::high_resolution_clock::now(), end = std::chrono::high_resolution_clock::now();
    chrono::duration<double> duration;

    double avarage_time, duration_sec;
    ofstream out;
    out.open("../heap_sort_results.txt");
    if (out.is_open())
    {
        for (int s : test_array_sizes) {
            avarage_time = 0;
            data_set_count = 0;
            ifstream in("../test_data_" + to_string(s) + ".txt");
            if (in.is_open())
            {
                while (getline(in, line))
                {
                    data_set_count++;
                    int* cur_array = new int[s];
                    n_start = 0;
                    el_number = 0;
                    for (int i = 0; i < line.size(); i++) {
                        if (line[i] == ' ') {
                            cur_array[el_number++] = (stoi(line.substr(n_start, i - n_start)));
                            n_start = i + 1;
                        }
                    }
                    start = chrono::high_resolution_clock::now();
                    heapSort(cur_array, s);
                    end = chrono::high_resolution_clock::now();
                    duration = end - start;

                    cout << data_set_count  << ") " << fixed << setprecision(7) << duration.count() << " sec" << endl;
                    /*for (int i = 0; i < s; i++) {
                        cout << cur_array[i] << " ";
                    }cout <<endl;*/
                    avarage_time += duration.count();
                    delete[] cur_array;
                    cur_array = nullptr;
                }
            }
            in.close();

            cout << endl << fixed << setprecision(7) << (avarage_time / data_set_count) << endl << "---------------------------------------------------------------------------------------" << endl << endl;
            out << s << ":" << fixed << setprecision(7) << (avarage_time / data_set_count) << endl;
        }
    }
    out.close();
}