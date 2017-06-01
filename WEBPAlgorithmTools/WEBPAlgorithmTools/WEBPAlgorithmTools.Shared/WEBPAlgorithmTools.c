#include "WEBPAlgorithmTools.h"
#include "../../examples/cwebp.h"

int WEBPEncode_internal(const char * imageSource, const char * imageDestination, const char *quality) {
	const char * string[6];
	string[0] = "cwebp";
	string[1] = "-q";
	string[2] = quality;
	string[3] = imageSource;
	string[4] = "-o";
	string[5] = imageDestination;
	return cwebp(6, string);
}


