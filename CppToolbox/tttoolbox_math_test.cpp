#include "tttoolbox_math.hpp"
#include <map>

#define BOOST_TEST_MODULE ToolboxMath
#define BOOST_TEST_DYN_LINK
#include <boost/test/unit_test.hpp>

using namespace TTToolbox;

const double xval[] = {1, 2, 3, 4, 5};
const double yval[] = {10, 40, 90, 100, 200};
const std::map<double,double> values = {
    {1.0, 10.0},
    {2.0, 40.0},
    {3.0, 90.0},
    {4.0, 100.0},
    {5.0, 200.0}
};

BOOST_AUTO_TEST_CASE(Estimate_Array_Lookup)
{
    BOOST_CHECK(10 == estimate(1, xval, yval, 5));
}

BOOST_AUTO_TEST_CASE(Estimate_Array_Interpolation)
{
    BOOST_CHECK(25 == estimate(1.5, xval, yval, 5));
}

BOOST_AUTO_TEST_CASE(Estimate_Array_Extrapolation)
{
    BOOST_CHECK(-5 == estimate(0.5, xval, yval, 5));
    BOOST_CHECK(700 == estimate(10, xval, yval, 5));
}


BOOST_AUTO_TEST_CASE(Estimate_Lookup)
{
    BOOST_CHECK(10 == estimate(1, values));
}

BOOST_AUTO_TEST_CASE(Estimate_Interpolation)
{
    BOOST_CHECK(25 == estimate(1.5, values));
}

BOOST_AUTO_TEST_CASE(Estimate_Extrapolation)
{
    BOOST_CHECK(-5 == estimate(0.5, values));
    BOOST_CHECK(700 == estimate(10, values));
}
