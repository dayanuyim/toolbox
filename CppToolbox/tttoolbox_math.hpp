#ifndef __TTTBOX_MATH_H
#define __TTTBOX_MATH_H

#include <stdexcept>
#include <map>

namespace TTToolbox{

size_t lessThan(double val, const double values[], size_t sz)
{
    if(sz == 0)
        throw std::invalid_argument("value array is empty");
            
    for(size_t i = 0; i < sz; ++i)
        if(val <= values[i])
            return i;
    return sz;
}

double estimate(double value, const double xval[], const double yval[], size_t sz)
{
    if(sz < 2)
        throw std::invalid_argument("value array should >= 2 to estimate");

    size_t i = lessThan(value, xval, sz);
    if( i == 0) i = 1;
    if( i >= sz) i = sz -1;

    double x1 = xval[i-1];
    double x2 = xval[i];
    double y1 = yval[i-1];
    double y2 = yval[i];

    return (y2 - y1) * (value - x1) / (x2 - x1) + y1;
}

double estimate(double value, const std::map<double,double> &values)
{
    if(values.size() < 2)
        throw std::invalid_argument("value array should >= 2 to estimate");

    //get the @it, which just larger than value
    std::map<double,double>::const_iterator it = values.begin();
    for(; it != values.end(); ++it){
        if(value <= it->first)
            break;
    }

    //check bordery
    if(it == values.begin()) ++it;
    if(it == values.end()) --it;

    --it;
    double x1 = it->first;
    double y1 = it->second;
    ++it;
    double x2 = it->first;
    double y2 = it->second;

    return (y2 - y1) * (value - x1) / (x2 - x1) + y1;
}

}//namespace
#endif
