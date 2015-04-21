#include <UnitTest++.h>
#include <iostream>
#include <string>

using namespace std;

SUITE(SuiteName)
{
	TEST(TestName)
	{
		CHECK(true);
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
