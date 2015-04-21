#include <UnitTest++.h>
#include <iostream>
#include <string>
#include "tttoolbox_misc.h"

using namespace std;

SUITE(SuiteName)
{
	TEST(strSplit_Test)
	{
		using TTToolbox::strSplit;

		CHECK_EQUAL(1, strSplit("2", ":").size());
		CHECK_EQUAL(2, strSplit("3:2", ":").size());
		CHECK_EQUAL(3, strSplit("1:55:02", ":").size());

		CHECK_EQUAL(2, strSplit("2.34", ":.").size());
		CHECK_EQUAL(3, strSplit("3:2.5", ":.").size());
		CHECK_EQUAL(4, strSplit("1:55:02.7", ":.").size());

		//special case
		CHECK_EQUAL(0, strSplit("", ":").size());
		CHECK_EQUAL(0, strSplit(":", ":").size());
		CHECK_EQUAL(0, strSplit("::", ":").size());
		CHECK_EQUAL(0, strSplit("::::", ":").size());
		CHECK_EQUAL(3, strSplit(":1:55:02.7:", ":").size());
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
