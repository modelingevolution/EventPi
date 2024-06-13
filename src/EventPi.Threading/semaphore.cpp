#include <fcntl.h> // For O_CREAT, O_RDWR
#include <semaphore.h>
#include <iostream>

// BUILD: g++ -shared -fPIC semaphore.cpp -o sem.so
extern "C" {
    sem_t* open_sem(const char* name, int oflag, mode_t mode, unsigned int value) {
        return sem_open(name, oflag | O_CREAT, mode, value);
    }
}
