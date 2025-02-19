#ifndef SERVER_H
#define SERVER_H

#include <stdlib.h>
#include <string.h>
#include <stdio.h>

// #define DEBUG_VERBOSITY

#ifdef DEBUG_VERBOSITY
#define DEBUG_PRINT(...) do { printf(__VA_ARGS__); fflush(stdout); } while(false)
#else
#define DEBUG_PRINT(...)
#endif

#define BUFFER_SIZE 4096

int str_ends_with(const char* str, const char* suffix)
{
    if (!str || !suffix)
    {
        return 0;
    }

    size_t lenstr = strlen(str);
    size_t lensuffix = strlen(suffix);
    if (lensuffix >  lenstr)
    {
        return 0;
    }

    return strncmp(str + lenstr - lensuffix, suffix, lensuffix) == 0;
}

#endif
