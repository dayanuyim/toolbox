#ifndef __TTTBOX_MISC_H
#define __TTTBOX_MISC_H

#include <cxxabi.h>
#include <unistd.h>
#include <typeinfo>
#include <memory>
#include <string>
#include <cstring>
#include <iostream>
#include <sstream>
#include <vector>

namespace TTToolbox{

using std::string;

/*************************************************
  sec sleep, like sleep() but supple 'double'
**************************************************/
inline
void ssleep(double sec)
{
	usleep(static_cast<useconds_t>(sec*1000000));
}

/*************************************************
  print out human-freidly TypeName 
**************************************************/
inline
std::string demangle(const char* mangled)
{
	int status;
	std::unique_ptr<char[], void (*)(void*)> result(
			abi::__cxa_demangle(mangled, 0, 0, &status), std::free);
	return result.get() ? std::string(result.get()) : "demangle name error";
}

template<class T>
std::string typeName(const T &t) { return demangle(typeid(t).name());}

template<class T>
std::string typeName() { return demangle(typeid(T).name());}


/*************************************************
  wapper function: strerror_r 
**************************************************/
inline
std::string strerror_r(int errnum)
{
	char buf[128];
	::strerror_r(errnum, buf, sizeof(buf));  //GNU return *cp; but XSI return int
	return buf;
}

/*************************************************
  wapper function: atof 
**************************************************/
inline
double atof(const string &str)
{
	return ::atof(str.c_str());
}

/*************************************************
  wapper function: atoi 
**************************************************/
inline
int atoi(const string &str)
{
	return ::atoi(str.c_str());
}

/*************************************************
  split string, veturn vector
**************************************************/
std::vector<string> strSplit(const string &str, const string &delimiter)
{
	std::vector<string> v;

	size_t begin = 0;
	for(size_t i = 0; i < str.length(); ++i){
		if(delimiter.find(str[i])!= string::npos){
			if(size_t len = i - begin)
				v.push_back(str.substr(begin, len));
			begin = i +1;
		}
	}

	//last,
	if(begin < str.length())
		v.push_back(str.substr(begin));

	return v;
}


/*************************************************
  set stream's locale, then recover later
**************************************************/
template<typename T>
class basic_locale_guard {
public:
	basic_locale_guard(std::basic_ios<T> &s, const std::locale &loc)
		:s_(s), old_loc_(s.imbue(loc))
	{
	}

	~basic_locale_guard() noexcept
	{
		try{ s_.imbue(old_loc_); }catch(...){}
	}
private:
	std::basic_ios<T> &s_;
	std::locale old_loc_;
};

using locale_guard = basic_locale_guard<char>;

/*************************************************
 * GetLineTypes
 *   support for Unix, Windows, or Mac
**************************************************/
enum class GetLineTypes{None = 0, Unix = 1, Win = 2, Mac = 4};

constexpr GetLineTypes operator|(GetLineTypes a, GetLineTypes b)
{
	return (GetLineTypes)((int)a|(int)b);
}

constexpr GetLineTypes operator&(GetLineTypes a, GetLineTypes b)
{
	return (GetLineTypes)((int)a&(int)b);
}

constexpr bool operator>=(GetLineTypes a, GetLineTypes b) //a contains b
{
	return (a & b) == b;
}

//has the operator any useful?
constexpr bool operator>(GetLineTypes a, GetLineTypes b)
{
	return a >= b && a != b;
}

// my getline version, support '\n', '\r\n', or '\r'.
/*************************************************
 * my getline version (ref ::getline(istream, string)
 * support for Unix, Windows, or Mac
 *  ret_type: return in which case the line is got.
 *  types: which type of using to get line
**************************************************/
inline
bool fgetline(std::istream &is, std::string &str,
        GetLineTypes *ret_type = 0,
		GetLineTypes types = GetLineTypes::Unix | GetLineTypes::Win | GetLineTypes::Mac)
{
	str.clear();
    if(ret_type) *ret_type = GetLineTypes::None;

	for(char c; is.get(c); ){

		//windows
		if(c == '\r' && types >= GetLineTypes::Win){
			if(is.peek() == '\n'){
				is.get(c);
                if(ret_type) *ret_type = GetLineTypes::Win;
				break;
			}
		}

		//mac
		if(c == '\r' && types >= GetLineTypes::Mac){
            if(ret_type) *ret_type = GetLineTypes::Mac;
			break;
        }

		//unix
		if(c == '\n' && types >= GetLineTypes::Unix){
            if(ret_type) *ret_type = GetLineTypes::Unix;
			break;
        }

		str += c;
	}

	return is || !str.empty();
}

/*************************************************
 * fgetlines: get file all lines
 *   is: istream
 *   ret_type: newline type of the istream
**************************************************/
std::vector<std::string> fgetlines(std::istream &is, GetLineTypes *ret_type = 0)
{
    std::vector<std::string> lines;

	for(string s; fgetline(is, s, ret_type); ret_type = 0) //get ret_type of first time
        lines.push_back(move(s));

    return lines;
}


}//namespace

#endif
