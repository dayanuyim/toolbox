#ifndef __TTTOOLBOX_STR_HPP
#define __TTTOOLBOX_STR_HPP

#include <string>
#include <cstring>

namespace TTToolbox{

/**********************************************
 * Wheter str is EndWith suffix
 **********************************************/
inline
bool strEndWith(const std::string &s, const char *suffix)
{
	size_t len = strlen(suffix);

	if(s.length() < len)
		return false;

	return s.compare(s.length() - len, len, suffix) == 0;
}

inline
bool strEndWith(const std::string &s, const std::string &suffix)
{
	return strEndWith(s, suffix.c_str());
}

/**********************************************
 * Wheter str is BeginWith prefix
 **********************************************/
inline
bool strBeginWith(const std::string &s, const char *prefix)
{
	size_t len = strlen(prefix);

	if(s.length() < len)
		return false;

	return s.compare(0, len, prefix) == 0;
}

inline
bool strBeginWith(const std::string &s, const std::string &prefix)
{
	return strBeginWith(s, prefix.c_str());
}

/**********************************************
 * definition of  whitespace
 **********************************************/
const char *whitespace = " \t\n\r\v\f";

/**********************************************
 * trim the begin of the string
 * @if you want to avoid innecessary copy, passing in move(string)
 **********************************************/
inline
std::string strTrimBegin(std::string s, const std::string &trim = whitespace)
{
    s.erase(0, s.find_first_not_of(trim));  //if 'find' return string::npos, it's ok
    return s;
}

/**********************************************
 * trim the end of the string
 * @if you want to avoid innecessary copy, passing in move(string)
 **********************************************/
inline
std::string strTrimEnd(std::string s, const std::string &trim = whitespace)
{
    s.erase(s.find_last_not_of(trim)+ 1);   //if 'find' return string::npos, it's ok. imply npos==(size_t)-1
    return s;
}

/**********************************************
 * trim both end of a string
 * @if you want to avoid innecessary copy, passing in move(string)
 **********************************************/
inline
std::string strTrim(std::string s, const std::string &trim = whitespace)
{
    s = strTrimBegin(move(s), trim);
    s = strTrimEnd(move(s), trim);
    return s;
}

}//namespace
#endif
