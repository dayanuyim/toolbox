#ifndef __TTTOOLBOX__PRINT_H
#define __TTTOOLBOX__PRINT_H

#include <iostream>
#include <iomanip>
#include <ostream>
#include <sstream>
#include <string>
#include <utility>
#include <ctime>
#include <cstring>
#include <memory>
#include <algorithm>

#include <tuple>
#include <vector>
#include <map>
#include <list>
#include <chrono>

namespace TTToolbox{

/***********************************
 * global toString()
************************************/
template<typename T>
std::string toString(const T &t)
{
	std::stringstream ss;
	ss << t;
	return ss.str();
}

/***********************************
  Stream out a tuple object

Copy From "The C++ Programming Language, 4/e",
	s.28.6.4, p.817
************************************/
template<size_t N>  //print element N, and following elements
struct print_tuple{
	template<typename... T>
	typename std::enable_if<(N <sizeof...(T))>::type
	print(std::ostream &os, const std::tuple<T...> &t)
	{
		os << ", " << std::get<N>(t);
		print_tuple<N+1>().print(os, t);
	}	

	template<typename... T>
	typename std::enable_if<!(N <sizeof...(T))>::type
	print(std::ostream &, const std::tuple<T...> &)   //empty tuple
	{
	}	
};

template<typename T0, typename... T>
std::ostream& operator<<(std::ostream& os, const std::tuple<T0, T...>& t) 
{
	os << "{" << std::get<0>(t);
	print_tuple<1>().print(os, t);
	os << "}";
	return os;
}

std::ostream& operator<<(std::ostream& os, const std::tuple<>&) //empty tuple
{
	return os << "{}";
}


/*************************************************
  ostream << Container
**************************************************/
//2nd argument just for duducing the right Container Type, 
//are there better way?
template<typename CharT, typename Cont, typename Elem>
std::basic_ostream<CharT>& printElement(std::basic_ostream<CharT> &os, const Cont &, const Elem &e)
{
	return os << e;
}

template<typename CharT, typename K, typename V, typename Elem>
std::basic_ostream<CharT>& printElement(std::basic_ostream<CharT> &os, const std::map<K,V>&, const Elem &e)
{
	return os << e.first << ':' << e.second;
}

template<typename CharT, typename Cont>
std::basic_ostream<CharT>& printContainer(std::basic_ostream<CharT> &os, const Cont &c)
{
	if(c.size() == 0)
		return os << "{}";

	auto it = c.cbegin();
	os << '{';
	printElement(os, c, *it);
	++it;

	for(; it != c.cend(); ++it){
		os << ", ";
		printElement(os, c, *it);
	}

	os << '}';

	return os;
}

/*************************************************
  ostream << vector
**************************************************/
template<typename CharT, typename T>
std::basic_ostream<CharT>& operator<<(std::basic_ostream<CharT> &os, const std::vector<T> &c)
{
	return printContainer(os, c);
}

/*************************************************
  ostream << list
**************************************************/
template<typename CharT, typename T>
std::basic_ostream<CharT>& operator<<(std::basic_ostream<CharT> &os, const std::list<T> &c)
{
	return printContainer(os, c);
}

/*************************************************
  ostream << map
**************************************************/
template<typename CharT, typename K, typename V>
std::basic_ostream<CharT>& operator<<(std::basic_ostream<CharT> &os, const std::map<K,V> &c)
{
	return printContainer(os, c);
}

/*************************************************
  toString(chrono::time_point, string fmt)
  fmt is the same as strftime(), but support %f for fragment
**************************************************/
inline
std::string getTimeStr(const struct tm &tm, const std::string &fmt)
{
    if(fmt.empty())
        return "";

    size_t len = std::max((size_t)32, fmt.length() * 3);
    std::unique_ptr<char[]> buf{new char[len]};
    
    strftime(buf.get(), len, fmt.c_str(), &tm);
    return buf.get();
}

inline
std::string getFragStr(double frag, size_t precision)
{
    std::stringstream ss_frag;
    ss_frag << std::fixed << std::setprecision(precision) << frag;

    std::string s = ss_frag.str();
    return s.substr(s.find('.') +1);
}

template <typename Clock, typename Duration>
std::string toString(const std::chrono::time_point<Clock,Duration> &tp,
        const std::string &fmt = "%Y-%m-%d %H:%M:%S.%ffffff")
{
    //*
    typedef typename Clock::duration duration_t;
    typedef typename Clock::period period_t;

    duration_t dur = tp.time_since_epoch();
    double sec_f = (double)dur.count() * period_t::num / period_t::den;
    time_t sec = sec_f;
    double frag = sec_f - sec;
    //time_t sec = dur.count() * period_t::num / period_t::den;
    //double frag = dur.count() % period_t::den;
    
    /*/
    time_t sec = Clock::to_time_t(tp);    //system_clock only
    double frag = 0;
    //*/
    
    struct tm tm;
    localtime_r(&sec, &tm);

    std::string text;

    size_t begin = 0;
    for(size_t pos; (pos = fmt.find("%f", begin)) != std::string::npos;){

        //non-frag string
        size_t len = pos - begin;
        if(len > 0)
            text += getTimeStr(tm, fmt.substr(begin, len));

        //frag string
        begin = pos +1;
        pos = fmt.find_first_not_of('f', begin);
        len = (pos == std::string::npos)?
            fmt.length() - begin:
            pos - begin;
        text += getFragStr(frag, len);

        begin = pos;
    }

    //rest non-frag string
    if(begin < fmt.length())
        text += getTimeStr(tm, fmt.substr(begin));

    return text;
}

/*************************************************
  ostream << time_point
**************************************************/
template<typename CharT, typename Clock, typename Duration>
std::basic_ostream<CharT>& operator<<(std::basic_ostream<CharT> &os,
        const std::chrono::time_point<Clock,Duration> &tp)
{
    return os << toString(tp);
}

}//namespace
#endif
