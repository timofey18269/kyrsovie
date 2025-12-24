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

void selectionSort(int a[], int n) {
    int cur_min_indx;
    for (int i = 0; i < n - 1; i++) {
        cur_min_indx = i;
        for (int j = i+1; j < n; j++) {
            if (a[j] < a[cur_min_indx]) {
                cur_min_indx = j;
            }
        }
        swap(a[cur_min_indx], a[i]);
    }
}

int main()
{
    const vector<int> test_array_sizes = { 10,20,50,100,200,500,1000,2000,5000,10000 };
    int data_set_count;
    string line;
    int n_start, el_number;
    auto start = chrono::high_resolution_clock::now(),end = chrono::high_resolution_clock::now();
    chrono::duration<double> duration;

    double avarage_time, duration_sec;
    ofstream out;
    out.open("../selection_sort_results.txt");
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
                    int*  cur_array = new int[s];
                    n_start = 0;
                    el_number = 0;
                    for (int i = 0; i < line.size(); i++) {
                        if (line[i] == ' ') {
                            cur_array[el_number++] = (stoi(line.substr(n_start, i-n_start)));
                            n_start = i + 1;
                        }
                    }
                    start = chrono::high_resolution_clock::now();
                    selectionSort(cur_array, s);
                    end = chrono::high_resolution_clock::now();
                    duration = end - start;

                    cout << data_set_count << ") " << fixed << setprecision(7) << duration.count() << " sec" << endl;
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

