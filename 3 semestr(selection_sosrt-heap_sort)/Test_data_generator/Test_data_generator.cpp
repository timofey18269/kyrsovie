#include <iostream>
#include <fstream>
#include <cstdlib>
#include <ctime>
#include <vector>
#include <string>
#include <random>
#include <chrono>
#include <iomanip>
#include <sstream>
#include <algorithm>

using namespace std;

void test_sort() {
    const vector<int> test_array_sizes = { 10,20,50,100,200,500,1000,2000,5000,10000 };
    int sorted_data_set_count;
    string line;
    int n_start, el_number;
    auto start = chrono::high_resolution_clock::now(), end = chrono::high_resolution_clock::now();
    chrono::duration<double> duration;

    double avarage_time, duration_sec;
    ofstream out;
    out.open("../c++_sort_results.txt");
    if (out.is_open())
    {
        for (int s : test_array_sizes) {
            avarage_time = 0;
            sorted_data_set_count = 0;
            ifstream in("../test_data_" + to_string(s) + ".txt");
            if (in.is_open())
            {
                while (getline(in, line))
                {
                    sorted_data_set_count++;
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
                    sort(cur_array, cur_array+s);
                    end = chrono::high_resolution_clock::now();
                    duration = end - start;

                    cout << sorted_data_set_count << ") " << fixed << setprecision(7) << duration.count() << " sec" << endl;
                    avarage_time += duration.count();

                    delete[] cur_array;
                    cur_array = nullptr;
                }
            }
            in.close();

            cout << endl << fixed << setprecision(7) << (avarage_time / sorted_data_set_count) << endl << "---------------------------------------------------------------------------------------" << endl << endl;
            out << s << ":" << fixed << setprecision(7) << (avarage_time / sorted_data_set_count) << endl;
        }
    }
    out.close();
}

int main()
{
    const vector<int> test_array_sizes = { 10,20,50,100,200,500,1000,2000,5000,10000 };
    const int data_set_count = 500;

    random_device rd;
    mt19937 gen(rd()); 
    uniform_int_distribution<> dist(INT_MIN , INT_MAX);

    ofstream out;

    int random_value;
    for (int s : test_array_sizes) {
        out.open("../test_data_" + to_string(s) + ".txt");
        for (int h = 0; h < data_set_count; h++) {
            for (int i = 0; i < s; i++) {
                random_value = dist(gen);
                out << random_value << " ";
            }
            if (h < data_set_count - 1) {
                out << endl;
            }
        }
        out.close();
    }

    test_sort();
}
