#include "WEBPAlgorithmTools.h"

__declspec(dllexport)int WEBPEncode(const char * imageSource, const char * imageDestination) {
	return WEBPEncode_internal(imageSource, imageDestination);
}