#include <UnitTest++.h>
#include <iostream>
#include <string>
#include "tttoolbox_str.hpp"

using namespace std;
using namespace TTToolbox;

SUITE(SuiteName)
{
	TEST(TestTrimBegin)
	{
        string s = "  aa  ";
		CHECK_EQUAL("aa  ", strTrimBegin(s));

        //move
        s = strTrimBegin(move(s));
		CHECK_EQUAL("aa  ", s);

        CHECK_EQUAL("", strTrimBegin(""));
        CHECK_EQUAL("", strTrimBegin(" "));
        CHECK_EQUAL("", strTrimBegin("  "));
        CHECK_EQUAL("", strTrimBegin(" \t"));
        CHECK_EQUAL("", strTrimBegin(" \t\v\f   "));
        CHECK_EQUAL("a", strTrimBegin("a"));
        CHECK_EQUAL("a\t\n", strTrimBegin("\t\na\t\n"));
        CHECK_EQUAL("a", strTrimBegin("\t\na"));
        CHECK_EQUAL("a\t\n", strTrimBegin("a\t\n"));
	}

	TEST(TestTrimEnd)
	{
        string s = "  aa  ";
		CHECK_EQUAL("  aa", strTrimEnd(s));

        //move
        s = strTrimEnd(move(s));
		CHECK_EQUAL("  aa", s);

        CHECK_EQUAL("", strTrimEnd(""));
        CHECK_EQUAL("", strTrimEnd(" "));
        CHECK_EQUAL("", strTrimEnd("  "));
        CHECK_EQUAL("", strTrimEnd(" \t"));
        CHECK_EQUAL("", strTrimEnd(" \t\v\f   "));
        CHECK_EQUAL("a", strTrimEnd("a"));
        CHECK_EQUAL("\t\na", strTrimEnd("\t\na\t\n"));
        CHECK_EQUAL("\t\na", strTrimEnd("\t\na"));
        CHECK_EQUAL("a", strTrimEnd("a\t\n"));
	}

	TEST(TestTrim)
	{
        string s = "  aa  ";
		CHECK_EQUAL("aa", strTrim(s));

        //move
        s = strTrim(move(s));
		CHECK_EQUAL("aa", s);

        CHECK_EQUAL("", strTrim(""));
        CHECK_EQUAL("", strTrim(" "));
        CHECK_EQUAL("", strTrim("  "));
        CHECK_EQUAL("", strTrim(" \t"));
        CHECK_EQUAL("", strTrim(" \t\v\f   "));
        CHECK_EQUAL("a", strTrim("a"));
        CHECK_EQUAL("a", strTrim("\t\na\t\n"));
        CHECK_EQUAL("a", strTrim("\t\na"));
        CHECK_EQUAL("a", strTrim("a\t\n"));
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
