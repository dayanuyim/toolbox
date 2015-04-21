#include <UnitTest++.h>
#include <iostream>
#include <sstream>
#include <string>
#include <vector>
#include <map>
#include "tttoolbox_print.hpp"

using namespace std;
using namespace TTToolbox;

SUITE(SuiteName)
{
	TEST(printToString)
	{
		auto a = make_tuple(1, 2, 3);
		CHECK_EQUAL("{1, 2, 3}", toString(a));
		
		vector<int> v1 = {1, 55, 2};
		CHECK_EQUAL("{1, 55, 2}", toString(v1));
	}

	TEST(printTuple)
	{
		auto a = make_tuple(1, 2, 3);
		auto b = make_tuple(1, "2", 3.0);

		ostringstream os;

		os << a;
		CHECK_EQUAL("{1, 2, 3}", os.str());
		os.str("");

		os << b;
		CHECK_EQUAL("{1, 2, 3}", os.str());
		os.str("");
	}

	TEST(printVector)
	{
		vector<int> v1 = {1, 55, 2};
		vector<int> v2 = {1};
		vector<int> v3 = {};

		ostringstream os;

		os << v1;
		CHECK_EQUAL("{1, 55, 2}", os.str());
		os.str("");

		os << v2;
		CHECK_EQUAL("{1}", os.str());
		os.str("");

		os << v3;
		CHECK_EQUAL("{}", os.str());
		os.str("");
	}

	TEST(printList)
	{
		list<int> v1 = {1, 55, 2};
		list<int> v3 = {};

		ostringstream os;

		os << v1;
		CHECK_EQUAL("{1, 55, 2}", os.str());
		os.str("");

		os << v3;
		CHECK_EQUAL("{}", os.str());
		os.str("");
	}

	TEST(printMap)
	{
		map<int,string> tab;
		tab[1] = "1";
		tab[2] = "2";
		
		map<int,string> empty;

		ostringstream os;

		os << tab;
		CHECK_EQUAL("{1:1, 2:2}", os.str());
		os.str("");

		os << empty;
		CHECK_EQUAL("{}", os.str());
		os.str("");
	}

    TEST(TestTimePoint)
    {
        using namespace std::chrono;

        cout << system_clock::now() << endl;
        cout << steady_clock::now() << endl;
        cout << high_resolution_clock::now() << endl;
    }
}

int main()
{
	try{
		UnitTest::RunAllTests();
	}
	catch(exception &ex){
		cerr << "system error: " << ex.what() << endl;
	}
	catch(...){
		cerr << "system error: " << "unknown" << endl;
	}

	return 0;
}
